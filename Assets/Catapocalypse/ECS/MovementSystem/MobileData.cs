using Unity.Entities;
using Unity.Mathematics;

namespace Catapocalypse.ECS.MovementSystem
{
    public partial struct MobileData : IComponentData
    {
        public double Speed;

        public struct Velocity : IComponentData
        {
            public float Value;
        }
    }
}