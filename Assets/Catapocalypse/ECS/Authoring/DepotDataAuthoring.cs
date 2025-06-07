using System;
using Unity.Entities;
using UnityEngine;

namespace Catapocalypse.ECS.GathererSystem
{
    [RequireComponent(typeof(InventorySystem.InventoryAuthoring))]
    public class DepotDataAuthoring : MonoBehaviour
    {
        public double Capacity = 2500;
        public class DataBaker : Baker<DepotDataAuthoring>
        {
            public override void Bake(DepotDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DepotData
                {
                    Capacity = authoring.Capacity
                });
            }
        }

        private void OnValidate()
        {
            GetComponent<InventorySystem.InventoryAuthoring>().Capacity = Capacity;
        }

        private void OnEnable()
        {
            GetComponent<InventorySystem.InventoryAuthoring>().Capacity = Capacity;
        }
    }
}