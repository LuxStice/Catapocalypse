using System;
using System.Collections.Generic;
using BovineLabs.Core.Collections;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace Catapocalypse.ECS.PrefabSystem
{
    public class PrefabDatabaseAuthoring : MonoBehaviour
    {
        [System.Serializable]
        public struct Entry
        {
            public string Id;
            public GameObject Prefab;
        }

        public List<Entry> Entries = new();
        private class PrefabDatabaseBaker : Baker<PrefabDatabaseAuthoring>
        {
            public override void Bake(PrefabDatabaseAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                
                var builder = new BlobBuilder(Allocator.Temp);
                ref var root = ref builder.ConstructRoot<PrefabDatabase.BlobData>();

                var map = builder.AllocateHashMap(ref root.PrefabById, authoring.Entries.Count);
                var buffer = AddBuffer<PrefabReference>(entity);

                foreach (var entry in authoring.Entries)
                {
                    var id = entry.Id;
                    var prefab = entry.Prefab;
                    
                    if(prefab is null) throw new System.NullReferenceException("Prefab is null");
                    if(string.IsNullOrEmpty(id)) throw new System.NullReferenceException("Prefab ID is null");
                    
                    var prefabEntity = GetEntity(prefab, TransformUsageFlags.Dynamic);
                    map.Add(new FixedString32Bytes(id), prefabEntity);
                    buffer.Add(new PrefabReference(id, prefabEntity));
                }

                var blobAsset = builder.CreateBlobAssetReference<PrefabDatabase.BlobData>(Allocator.Persistent);
                builder.Dispose();

                AddComponent(entity, new PrefabDatabase() { Blob = blobAsset });
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
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

            foreach (string guid in prefabGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if(prefab is null) throw new System.NullReferenceException("Prefab is null");
                if (!prefab.TryGetComponent(out PrefabAuthoring prefabAuthoring)) continue;
                
                Entries.Add(new Entry
                {
                    Id = prefabAuthoring.PrefabID,
                    Prefab = prefab
                });
            }
        }
    }
}