using Unity.Entities;
using Unity.Physics;
using Unity.Properties;

namespace Catapocalypse.ECS.RaycastSystem
{
    public struct MouseRaycastHit : IComponentData
    {
        [CreateProperty]
        public bool HasHit => Value.Equals(default);
        public RaycastHit Value;

        public override bool Equals(object obj)
        {
            return Value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}