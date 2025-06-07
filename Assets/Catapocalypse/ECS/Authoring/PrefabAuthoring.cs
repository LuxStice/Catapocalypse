using System;
using Unity.Entities;
using UnityEngine;

namespace Catapocalypse.ECS.PrefabSystem
{
    public class PrefabAuthoring : MonoBehaviour
    {
        public string PrefabID;
        public int PrefabHash;
        private class PrefabAuthoringBaker : Baker<PrefabAuthoring>
        {
            public override void Bake(PrefabAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                //AddComponent(entity, new Prefab());
                AddComponent(entity, new PrefabID(authoring.PrefabID));
            }
        }

        private void OnValidate()
        {
            PrefabHash = PrefabID.GetHashCode();
        }
    }
}