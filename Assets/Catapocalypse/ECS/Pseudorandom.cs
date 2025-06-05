using Unity.Entities;

namespace Catapocalypse.ECS
{
    public struct Pseudorandom : IComponentData
    {
        public Unity.Mathematics.Random Random;
    }
}