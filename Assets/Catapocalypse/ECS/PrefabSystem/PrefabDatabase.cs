using BovineLabs.Core.Collections;
using Unity.Collections;
using Unity.Entities;

namespace Catapocalypse.ECS.PrefabSystem
{
    public struct PrefabDatabase : IComponentData
    {
        public BlobAssetReference<BlobData> Blob;
        public struct BlobData
        {
            public BlobHashMap<FixedString32Bytes, Entity> PrefabById;

            public bool TryGetPrefab(FixedString32Bytes key, out Entity prefab)
            {
                prefab = Entity.Null;

                if (!PrefabById.ContainsKey(key)) return false;
                
                prefab = PrefabById[key];
                return true;
            }
        }
    }

}