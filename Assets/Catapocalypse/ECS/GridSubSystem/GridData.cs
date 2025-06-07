using Unity.Entities;
using Unity.Mathematics;

namespace Catapocalypse.ECS.GridSubSystem
{
    public struct GridData : IComponentData
    {
        public int2 Size;
        public int Width => Size.x;
        public int Height => Size.y;
        //public float3 Origin => localTransform.ValueRO.Position;
        public float CellSize;
        //private readonly RefRO<LocalTransform> localTransform;
    }
}