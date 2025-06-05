using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Catapocalypse.ECS.MovementSystem
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(AddRandomNodeSystem))]
    public partial struct UpdatePathStateSystem : ISystem
    {
        private EntityQuery query;
        private ComponentLookup<PathSeeker> pathLookup;
        private BufferLookup<Path.Node> pathNodeLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var queryDesc = new EntityQueryDesc()
            {
                All = new[] { ComponentType.ReadOnly<Path.Node>(), ComponentType.ReadOnly<PathSeeker>(),  },
                Options = EntityQueryOptions.IgnoreComponentEnabledState
            };

            query = state.GetEntityQuery(queryDesc);

            pathLookup = SystemAPI.GetComponentLookup<PathSeeker>();
            pathNodeLookup = SystemAPI.GetBufferLookup<Path.Node>();
            
            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = query.ToEntityArray(Allocator.TempJob);
            
            pathLookup.Update(ref state);
            pathNodeLookup.Update(ref state);
            
            var ecb = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            var job = new UpdatePathStateJob
            {
                Entities = entities.AsReadOnly(),
                PathNodesLookup = pathNodeLookup,
                PathLookup = pathLookup,
                ecb = ecb.AsParallelWriter()
            }.Schedule(entities.Length, 32, state.Dependency);
            
            state.Dependency = JobHandle.CombineDependencies(state.Dependency, job);

            entities.Dispose(job);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
        
        [BurstCompile]
        private struct UpdatePathStateJob : IJobParallelFor
        {
            public NativeArray<Entity>.ReadOnly Entities;
            
            [ReadOnly] public BufferLookup<Path.Node> PathNodesLookup;
            [ReadOnly] public ComponentLookup<PathSeeker> PathLookup;

            public EntityCommandBuffer.ParallelWriter ecb;
            
            public void Execute(int index)
            {
                var entity = Entities[index];

                var nodes = PathNodesLookup[entity];
                var hasNodes = !nodes.IsEmpty;
                var isComponentEnabled = PathLookup.IsComponentEnabled(entity);

                if (isComponentEnabled != hasNodes)
                    ecb.SetComponentEnabled<PathSeeker>(index, entity, hasNodes);
                
                ecb.SetComponent(index, entity, new PathSeeker.State
                {
                    CurrentNodePosition = (!nodes.IsEmpty) ? nodes[0].Value : float3.zero,
                    NextNodePosition = (nodes.Length >= 2) ? nodes[1].Value : float3.zero
                });
            }
        }
    }
}