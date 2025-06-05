using System.Collections.Generic;
using UnityEngine;

namespace Catapocalypse.ECS.GathererSystem
{
    [CreateAssetMenu(fileName = "Resource Node", menuName = "Catapocalypse/Resource Node Settings", order = 0)]
    public class ResourceNodeSettings : ScriptableObject
    {
        public List<ResourceNode.AvailableResource> Resources = new();
    }
}