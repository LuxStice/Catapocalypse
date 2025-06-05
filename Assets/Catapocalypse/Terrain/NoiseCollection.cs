using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Catapocalypse.Terrain
{
    [CreateAssetMenu(fileName = "Noise Collection", menuName = "Catapocalypse/Terrain/Noise Collection", order = 0)]
    public class NoiseCollection : ScriptableObject
    {
        public List<NoiseLayer> Layers = new();
    }
}