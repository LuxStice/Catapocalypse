using Unity.Collections;
using Unity.Entities;

namespace Catapocalypse.ECS.PrefabSystem
{
    public struct PrefabReference : IBufferElementData
    {
        public FixedString32Bytes Key;
        public Entity Value;

        public PrefabReference(FixedString32Bytes key, Entity value)
        {
            Key = key;
            Value = value;
        }
    }
}