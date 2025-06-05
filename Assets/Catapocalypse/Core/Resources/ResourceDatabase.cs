using System;
using System.Collections.Generic;
using UnityEngine;

namespace Catapocalypse.Core.Resources
{
    [CreateAssetMenu(fileName = "Resource Database", menuName = "Catapocalypse/Resource Database", order = 0)]
    public class ResourceDatabase : ScriptableObject
    {
        public List<ResourceDescriptor> Resources = new();
        
        public Dictionary<ResourceID, ResourceDescriptor> resourcesById { get; private set; } = new();

        private void OnValidate()
        {
            resourcesById.Clear();
            
            for (int i = 0; i < Resources.Count; i++)
            {
                resourcesById.Add(new ResourceID((uint)i), Resources[i]);
            }
        }
    }

    [Serializable]
    public struct ResourceDescriptor
    {
        public string Name;
        public string Description;
    }
}