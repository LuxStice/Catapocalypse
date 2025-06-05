using System;
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
    [UpdateInGroup(typeof(TransformSystemGroup))]
    public partial struct MoveAlongPathSystem : ISystem
    {
        private EntityQuery query;
        private ComponentLookup<LocalTransform> localTransformLookup;
        private ComponentLookup<MobileData> mobileLookup;
        private BufferLookup<Path.Node> pathNodeLookup;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<LocalTransform,MobileData,PathSeeker>()
                .Build(ref state);

            localTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
            mobileLookup = SystemAPI.GetComponentLookup<MobileData>();
            pathNodeLookup = SystemAPI.GetBufferLookup<Path.Node>();
            
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = query.ToEntityArray(Allocator.TempJob);
            
            localTransformLookup.Update(ref state);
            mobileLookup.Update(ref state);
            pathNodeLookup.Update(ref state);

            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            var job = new MoveAlongPathJob
            {
                Entities = entities.AsReadOnly(),
                TransformLookup = localTransformLookup,
                MobileLookup = mobileLookup,
                PathNodesLookup = pathNodeLookup,
                ecb = ecb.AsParallelWriter(),
                deltaTime = SystemAPI.Time.DeltaTime
            }.Schedule(entities.Length, 32, state.Dependency);
            
            state.Dependency = JobHandle.CombineDependencies(state.Dependency, job);

            entities.Dispose(job);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        private struct MoveAlongPathJob : IJobParallelFor
        {
            public NativeArray<Entity>.ReadOnly Entities;
            
            [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
            [ReadOnly] public ComponentLookup<MobileData> MobileLookup;
            [ReadOnly] public BufferLookup<Path.Node> PathNodesLookup;

            public EntityCommandBuffer.ParallelWriter ecb;
            public float deltaTime;
            
            public void Execute(int index)
            {
                var entity = Entities[index];
                var nodesRO = PathNodesLookup[entity];

                if (nodesRO.IsEmpty) throw new Exception("There are no nodes for the agent to move along.");
                
                var nodesWO = ecb.SetBuffer<Path.Node>(index, entity);
                nodesWO.CopyFrom(nodesRO);
                
                var mobile =  MobileLookup[entity];
                var transform =  TransformLookup[entity];
                
                var traveledThisFrame = deltaTime * mobile.Speed;
                
                while (traveledThisFrame > 0)
                {
                    var nodePosition =  nodesWO[0].Value;
                    var distanceToNode = math.distance(nodePosition, transform.Position);
                    var directionToNode = math.normalize(nodePosition - transform.Position);

                    if (distanceToNode <= traveledThisFrame)
                    {
                        transform.Position = nodePosition;
                        traveledThisFrame -= distanceToNode;
                        nodesWO.RemoveAt(0);
                    }
                    else
                    {
                        transform.Position += directionToNode * (float)traveledThisFrame;
                        break;
                    }
                }
                ecb.SetComponent(index, entity, transform);
                ecb.SetComponent(index, entity, new MobileData.Velocity{Value = deltaTime * (float)mobile.Speed});
            }
        }
    }
}