using System;
using Unity.Mathematics;

namespace Catapocalypse.Terrain
{
    [Serializable]
    public struct NoiseLayer
    {
        public string name;
        public NoiseData data;
        
        [Serializable]
        public struct NoiseData
        {
            public float2 scale;
            public float2 offset;
            public int octaves;
            public float frequency;
            public float amplitude;

            public static NoiseData Null => new(new float2(1), float2.zero); 

            public NoiseData(float2 scale, float2 offset)
            {
                this.scale = scale;
                this.offset = offset;
                this.octaves = 1;
                this.frequency = 1;
                this.amplitude = 1;
            }
        }
    }
}