using Unity.Collections;
using Unity.Entities;

namespace Catapocalypse.ECS.PrefabSystem
{
    public struct PrefabID : IComponentData
    {
        public FixedString32Bytes Value;

        public PrefabID(FixedString32Bytes value)
        {
            Value = value;
        }
    }
}