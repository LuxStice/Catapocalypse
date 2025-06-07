using Unity.Entities;

namespace Catapocalypse.ECS
{
    public struct Pseudorandom : IComponentData
    {
        public Unity.Mathematics.Random Random;

        public static uint GetNewSeed()
        {
            var seed = ((uint)UnityEngine.Random.Range(0, 65536) << 16) | (uint)UnityEngine.Random.Range(0, 65536);
            
            seed += int.MaxValue;
            return seed;
        }
    }
}