using Catapocalypse.ECS.InventorySystem;
using Unity.Entities;
using UnityEngine;

namespace Catapocalypse.ECS.GathererSystem
{
    [RequireComponent(typeof(InventoryAuthoring))]
    public class ResourceNodeAuthoring : MonoBehaviour
    {
        public ResourceNodeSettings Settings;
        public class ResourceNodeBaker : Baker<ResourceNodeAuthoring>
        {
            public override void Bake(ResourceNodeAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<ResourceNode>(entity, authoring.Settings.Node);
            }
        }
    }
}