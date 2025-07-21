using Unity.Entities;
using UnityEngine;

namespace Catapocalypse.Terrain.Authoring
{
    public class MarchingCubesAuthoring : MonoBehaviour
    {
        public MarchingCubesSettings settings;

        class Baker : Baker<MarchingCubesAuthoring>
        {
            public override void Bake(MarchingCubesAuthoring authoring)
            {
                AddComponent(new MarchingCubesChunk { ChunkIndex = int3.zero });

                // Store settings as a singleton
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponentObject(entity, authoring.settings);
            }
        }
    }
}