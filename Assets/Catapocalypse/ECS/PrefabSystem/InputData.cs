using BovineLabs.Core.Input;
using Unity.Entities;

namespace Catapocalypse.ECS.PrefabSystem
{
    public partial struct InputData : IComponentData
    {
        [InputAction]
        [InputActionDown]
        public bool SpawnPrefabDown;
        [InputAction]
        public bool SpawnPrefab;
        [InputActionUp]
        public bool SpawnPrefabUp;
    }
}