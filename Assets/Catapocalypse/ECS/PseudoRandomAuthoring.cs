using System;
using Unity.Entities;
using UnityEngine;

namespace Catapocalypse.ECS
{
    public class PseudoRandomAuthoring : MonoBehaviour
    {
        public uint seed;
        private class PseudoRandomBaker : Baker<PseudoRandomAuthoring>
        {
            public override void Bake(PseudoRandomAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                
                AddComponent(entity, new Pseudorandom
                {
                    Random = new Unity.Mathematics.Random(authoring.seed)
                });
            }
        }

        private void OnValidate()
        {
            seed = GetNewSeed();
        }

        public static uint GetNewSeed()
        {
            var seed = ((uint)UnityEngine.Random.Range(0, 65536) << 16) | (uint)UnityEngine.Random.Range(0, 65536);
            
            seed += int.MaxValue;
            return seed;
        }
    }
}