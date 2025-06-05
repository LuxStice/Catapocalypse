using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Catapocalypse.ECS.MovementSystem
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct DebugDrawSystem : ISystem
    {
        private EntityQuery query;
        private BufferLookup<Path.Node> pathNodeLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var queryDesc = new EntityQueryDesc()
            {
                All = new[] { ComponentType.ReadOnly<Path.Node>() },
            };

            query = state.GetEntityQuery(queryDesc);
            
            pathNodeLookup = SystemAPI.GetBufferLookup<Path.Node>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entities = query.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                var nodes = SystemAPI.GetBuffer<Path.Node>(entity);
                var transform = SystemAPI.GetComponent<LocalTransform>(entity);
                
                if(nodes.IsEmpty) continue;
                
                Debug.DrawLine(transform.Position, nodes[0].Value, Color.white);

                for (int i = 0; i < nodes.Length-1; i++)
                {
                    var nodePosition = nodes[i].Value;
                    var nextPosition = nodes[i+1].Value;
                    Debug.DrawLine(nodePosition, nextPosition, Color.green);
                }
            }
        }
    }
}