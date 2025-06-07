using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Catapocalypse.ECS.MovementSystem
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(AddRandomNodeSystem))]
    public partial struct UpdatePathStateSystem : ISystem
    {
        private EntityQuery query;
        private ComponentLookup<PathSeeker> pathLookup;
        private ComponentLookup<LocalTransform> localTransformLookup;
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
            localTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
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
            localTransformLookup.Update(ref state);

            var job = new UpdatePathStateJob
            {
                Entities = entities.AsReadOnly(),
                PathNodesLookup = pathNodeLookup,
                PathLookup = pathLookup,
                TransformLookup = localTransformLookup,
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
            [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;

            public EntityCommandBuffer.ParallelWriter ecb;
            
            public void Execute(int index)
            {
                var entity = Entities[index];

                var nodes = PathNodesLookup[entity];
                var hasNodes = !nodes.IsEmpty;
                var isComponentEnabled = PathLookup.IsComponentEnabled(entity);
                var transform = TransformLookup[entity];

                if (isComponentEnabled != hasNodes)
                    ecb.SetComponentEnabled<PathSeeker>(index, entity, hasNodes);

                float distanceToNextNode = (!nodes.IsEmpty) ? math.distance(transform.Position, nodes[0].Value) : -1;
                float distanceToFinalNode = distanceToNextNode; 

                for (int i = 1; i < nodes.Length-1; i++)
                {
                    distanceToNextNode += math.distance(nodes[1].Value, nodes[i+1].Value);
                }
                
                ecb.SetComponent(index, entity, new PathSeeker.State
                {
                    CurrentNodePosition = (!nodes.IsEmpty) ? nodes[0].Value : float3.zero,
                    NextNodePosition = (nodes.Length >= 2) ? nodes[1].Value : float3.zero,
                    DistanceToNextNode = distanceToNextNode,
                    DistanceToFinalNode = distanceToFinalNode
                });
            }
        }
    }
}