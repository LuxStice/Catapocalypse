using Unity.Entities;
using UnityEngine;

namespace Catapocalypse.ECS.PrefabSystem
{
    public class PrefabAuthoring : MonoBehaviour
    {
        public string PrefabID;
        private class PrefabAuthoringBaker : Baker<PrefabAuthoring>
        {
            public override void Bake(PrefabAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new Prefab());
                AddComponent(entity, new PrefabID(authoring.PrefabID));
            }
        }
    }
}