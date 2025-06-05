using Unity.Entities;

namespace Catapocalypse.ECS.GathererSystem
{
    public struct ResourceGeneratorData : IComponentData
    {
        public struct Data : IBufferElementData
        {
            public uint ResourceID;
            public double Rate;
        }
    }
}