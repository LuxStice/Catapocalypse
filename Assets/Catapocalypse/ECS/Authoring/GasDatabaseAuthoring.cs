using System.Collections.Generic;
using Catapocalypse.Core.Resources;
using Catapocalypse.ECS.GasSubSystem;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace Catapocalypse.ECS.Authoring
{
    public class GasDatabaseAuthoring : MonoBehaviour
    {
        public List<GasDescriptor> Entries = new();
        private Dictionary<GasID, GasDescriptor> map = new();
        public class GasDatabaseBaker : Baker<GasDatabaseAuthoring>
        {
            public override void Bake(GasDatabaseAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new GasDatabase
                {
                    Blob = GasDatabase.BlobData.BuildBlob(authoring.map)
                });
            }
        }

        private void OnValidate()
        {
            UpdateEntries();
        }

        private void OnEnable()
        {     
            UpdateEntries();
        }

        private void UpdateEntries()
        {  
            Entries.Clear();
            string[] prefabGuids = AssetDatabase.FindAssets("t:GasDescriptor");

            foreach (string guid in prefabGuids)
            {
                var descriptor = AssetDatabase.LoadAssetAtPath<GasDescriptor>(AssetDatabase.GUIDToAssetPath(guid));
                
                Entries.Add(descriptor);
            }
        }
    }
}