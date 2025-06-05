using System;
using BovineLabs.Core.Input;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using Ray = Unity.Physics.Ray;

namespace Catapocalypse.ECS.RaycastSystem
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct MouseRaycastSystem : ISystem
    {
        private Entity dataEntity;
        
        public void OnCreate(ref SystemState state)
        {
            dataEntity = state.EntityManager.CreateEntity();

            state.EntityManager.AddComponent<MouseRaycastHit>(dataEntity);
            
            state.RequireForUpdate<InputCommon>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var inputCommon = SystemAPI.GetSingleton<InputCommon>();

            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            var ray = inputCommon.CameraRay;

            var rayInput = new Unity.Physics.RaycastInput
            {
                Start = ray.Origin,
                End = ray.Origin + ray.Displacement * 1000f,
                Filter = CollisionFilter.Default
            };

            if (physicsWorld.CastRay(rayInput, out var hit))
            {
                var result = new MouseRaycastHit
                {
                    Value = hit,
                };
                SystemAPI.SetSingleton(result);
            }
            else
            {
                var result = new MouseRaycastHit
                {
                    Value = default
                };
                SystemAPI.SetSingleton(result);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}