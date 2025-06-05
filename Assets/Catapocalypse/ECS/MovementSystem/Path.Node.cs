using Catapocalypse.ECS.EntityStateSystem;
using Unity.Entities;
using Unity.Mathematics;

namespace Catapocalypse.ECS.MovementSystem
{
    public struct Path
    {
        public struct Node : IBufferElementData
        {
            public float3 Value;
        }
    }
}