using Unity.Entities;

namespace Catapocalypse.ECS.InventorySystem
{
    public struct Inventory : IComponentData
    {
        public uint Size;

        public struct State : IComponentData
        {
            public uint ContainedItems;
        }
        public struct Item : IBufferElementData
        {
            public uint ItemID;
            public uint Count;
        }
    }
}