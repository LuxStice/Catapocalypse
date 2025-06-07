using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Catapocalypse.ECS.PrefabSystem
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class SpawnSelectedPrefabSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var prefabDatabase = SystemAPI.GetSingleton<PrefabDatabase>();
            var prefabCollection = SystemAPI.GetSingletonBuffer<PrefabReference>();
            
            var input = SystemAPI.GetSingleton<InputData>();
            //var prefab = prefabDatabase.Blob.Value.PrefabById["agent"];
            if(!TryFindPrefab(prefabCollection, "agent", out var prefab)) throw new System.Exception("Prefab agent not found.");

            if (input.SpawnPrefabDown)
            {
                var instantiated = EntityManager.Instantiate(prefab);
                
                EntityManager.SetComponentData(instantiated, new Pseudorandom()
                {
                    Random = new Random(Pseudorandom.GetNewSeed())
                });
            }
        }

        private static bool TryFindPrefab(DynamicBuffer<PrefabReference> buffer, FixedString32Bytes key, out Entity prefab)
        {
            foreach (var prefabReference in buffer)
            {
                if(prefabReference.Key != key) continue;
                
                prefab = prefabReference.Value;
                return true;
            }
            
            prefab = Entity.Null;
            return false;
        }
    }
}