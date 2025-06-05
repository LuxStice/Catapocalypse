using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Catapocalypse.ECS.PrefabSystem
{
    public class PrefabDatabaseAuthoring : MonoBehaviour
    {
        public List<GameObject> Prefabs;
        private class PrefabDatabaseBaker : Baker<PrefabDatabaseAuthoring>
        {
            public override void Bake(PrefabDatabaseAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                var buffer = AddBuffer<PrefabReference>(entity);
                AddComponent<PrefabDatabase>(entity);

                foreach (var prefab in authoring.Prefabs)
                {
                    DependsOn(prefab);
                    
                    var prefabEntity = GetEntity(prefab, TransformUsageFlags.None);
                    buffer.Add(new PrefabReference(prefabEntity));
                }
            }
        }
    }
}