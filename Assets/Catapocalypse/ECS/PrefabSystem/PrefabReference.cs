using Unity.Entities;

namespace Catapocalypse.ECS.PrefabSystem
{
    public struct PrefabReference : IBufferElementData
    {
        public Entity Value;

        public PrefabReference(Entity value)
        {
            Value = value;
        }
    }
}