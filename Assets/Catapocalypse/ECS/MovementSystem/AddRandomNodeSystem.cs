using Catapocalypse.ECS.EntityStateSystem;
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
    public partial struct AddRandomNodeSystem : ISystem
    {
        private EntityQuery query;
        
        private ComponentLookup<LocalTransform> localTransformLookup;
        private ComponentLookup<Pseudorandom> pseudorandomLookup;
        private BufferLookup<Path.Node> pathNodeLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var queryDesc = new EntityQueryDesc()
            {
                All = new[] { ComponentType.ReadOnly<Path.Node>(), ComponentType.ReadOnly<Pseudorandom>(), ComponentType.ReadOnly<LocalTransform>(), },
            };

            query = state.GetEntityQuery(queryDesc);
            
            localTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
            pseudorandomLookup = SystemAPI.GetComponentLookup<Pseudorandom>();
            pathNodeLookup = SystemAPI.GetBufferLookup<Path.Node>();
            
            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = query.ToEntityArray(Allocator.TempJob);
            var ecb = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            
            localTransformLookup.Update(ref state);
            pseudorandomLookup.Update(ref state);
            pathNodeLookup.Update(ref state);

            var job = new AddRandomNodeJob
            {
                Entities = entities.AsReadOnly(),
                TransformLookup = localTransformLookup,
                PseudorandomsLookup = pseudorandomLookup,
                PathNodesLookup = pathNodeLookup,
                ecb = ecb.AsParallelWriter()
            }.Schedule(entities.Length, 32, state.Dependency);
            
            state.Dependency = JobHandle.CombineDependencies(state.Dependency, job);

            entities.Dispose(job);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
        private struct AddRandomNodeJob : IJobParallelFor
        {
            public NativeArray<Entity>.ReadOnly Entities;
            [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
            [ReadOnly] public ComponentLookup<Pseudorandom> PseudorandomsLookup;
            [ReadOnly] public BufferLookup<Path.Node> PathNodesLookup;

            public EntityCommandBuffer.ParallelWriter ecb;
            
            public void Execute(int index)
            {
                var entity = Entities[index];
                var nodesRO = PathNodesLookup[entity];
                
                if(nodesRO.Length >= 3) return;
                
                var nodesWO = ecb.SetBuffer<Path.Node>(index, entity);
                nodesWO.CopyFrom(nodesRO);
                
                var pseudorandom = PseudorandomsLookup[entity];

                for (int i = 0; i < 3 - nodesRO.Length; i++)
                {
                    var direction = pseudorandom.Random.NextFloat2Direction();
                    var distance = pseudorandom.Random.NextFloat(5f,20f);

                    var translation = new float3(direction.x * distance, 0, direction.y * distance);
                    var nodePosition = translation + TransformLookup[entity].Position;

                    nodesWO.Add(new Path.Node
                    {
                        Value = nodePosition
                    });
                }
                
                //Agent preparation
                ecb.SetComponent(index, entity, pseudorandom);
            }
        }
    }
}