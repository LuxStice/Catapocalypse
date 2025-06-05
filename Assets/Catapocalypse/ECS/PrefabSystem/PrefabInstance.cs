using Unity.Collections;
using Unity.Entities;

namespace Catapocalypse.ECS.PrefabSystem
{
    public struct PrefabInstance : IComponentData
    {
        public FixedString32Bytes PrefabName;
    }
}