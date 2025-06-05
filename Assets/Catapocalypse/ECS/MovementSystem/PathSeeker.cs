using Catapocalypse.ECS.EntityStateSystem;
using Unity.Entities;
using Unity.Mathematics;

namespace Catapocalypse.ECS.MovementSystem
{
    public struct PathSeeker : IComponentData, IEnableableComponent
    {
        public struct State : IComponentData
        {
            public float3 CurrentNodePosition;
            public float3 NextNodePosition;
        }
    }
}