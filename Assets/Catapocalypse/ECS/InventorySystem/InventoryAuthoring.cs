using Unity.Entities;
using UnityEngine;

namespace Catapocalypse.ECS.InventorySystem
{
    public class InventoryAuthoring : MonoBehaviour
    {
        [Range(0, 1024)]
        public uint Size = 64;
        
        public class InventoryDataBaker : Baker<InventoryAuthoring>
        {
            public override void Bake(InventoryAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Inventory { Size = authoring.Size });
                AddComponent(entity, new Inventory.State());
                AddBuffer<Inventory.Item>(entity);
            }
        }
    }
}