using Unity.Entities;
using UnityEngine;

namespace Catapocalypse.ECS.GathererSystem
{
    public class ResourceNodeAuthoring : MonoBehaviour
    {
        public ResourceNodeSettings Settings;
        public class ResourceNodeBaker : Baker<ResourceNodeAuthoring>
        {
            public override void Bake(ResourceNodeAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<ResourceNode>(entity, new ResourceNode
                {
                    Depleted = false
                });
                AddComponent<ResourceNode.State>(entity);
                
                var buffer = AddBuffer<ResourceNode.AvailableResource>(entity);

                foreach (var resource in authoring.Settings.Resources)
                {
                    buffer.Add(new ResourceNode.AvailableResource
                    {
                        ResourceID = resource.ResourceID,
                        Rate = resource.Rate
                    });
                }
            }
        }
    }
}