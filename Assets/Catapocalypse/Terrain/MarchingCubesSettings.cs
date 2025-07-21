using UnityEngine;
using Unity.Mathematics;

namespace Catapocalypse.Terrain
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Catapocalypse/Terrain/Marching Cubes Settings")]
    public class MarchingCubesSettings : ScriptableObject
    {
        public float isoLevel = 0.5f;
        public int3 chunkSize = new int3(32, 32, 32);
        public float voxelSize = 1f;
        public ComputeShader marchingCubesCompute;
    }
}