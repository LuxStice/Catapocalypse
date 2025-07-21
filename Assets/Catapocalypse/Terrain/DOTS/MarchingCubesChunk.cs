using Unity.Entities;
using Unity.Mathematics;

namespace Catapocalypse.Terrain.DOTS
{
    public struct MarchingCubesChunk : IComponentData
    {
        public int3 ChunkIndex;
    }
}