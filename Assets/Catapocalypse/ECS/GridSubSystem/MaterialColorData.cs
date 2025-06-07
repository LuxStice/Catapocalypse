using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Catapocalypse.ECS.GridSubSystem
{
    [MaterialProperty("_BaseColor")]
    public struct MaterialColorData : IComponentData
    {
        public float4 Value;
    }
}