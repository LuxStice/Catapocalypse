using Unity.Entities;
using UnityEngine;

namespace Catapocalypse.ECS.InventorySystem
{
    public class InventoryAuthoring : MonoBehaviour
    {
        public double Capacity = 64;
        
        public class InventoryDataBaker : Baker<InventoryAuthoring>
        {
            public override void Bake(InventoryAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Inventory { Capacity = authoring.Capacity });
                AddBuffer<Inventory.Item>(entity);
            }
        }
    }
}