using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Catapocalypse.ECS.InventorySystem
{
    public partial struct CalculateInventorySumSystem : ISystem
    {
        private EntityQuery query;
        
        private BufferLookup<Inventory.Item> inventoryItemLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var queryDesc = new EntityQueryDesc()
            {
                All = new[] { ComponentType.ReadOnly<Inventory>(), ComponentType.ReadOnly<Inventory.State>(), ComponentType.ReadOnly<Inventory.Item>(), },
            };

            query = state.GetEntityQuery(queryDesc);
            
            inventoryItemLookup = SystemAPI.GetBufferLookup<Inventory.Item>();
            
            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = query.ToEntityArray(Allocator.TempJob);
            var ecb = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
                        
            inventoryItemLookup.Update(ref state);

            var job = new CalculateInventorySumJob
            {
                Entities = entities.AsReadOnly(),
                InventoryItemsLookup = inventoryItemLookup,
                ecb = ecb.AsParallelWriter()
            }.Schedule(entities.Length, 32, state.Dependency);
            
            state.Dependency = JobHandle.CombineDependencies(state.Dependency, job);

            entities.Dispose(job);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        private struct CalculateInventorySumJob : IJobParallelFor
        {
            public NativeArray<Entity>.ReadOnly Entities;

            [ReadOnly] public BufferLookup<Inventory.Item> InventoryItemsLookup;

            public EntityCommandBuffer.ParallelWriter ecb;
            
            public void Execute(int index)
            {
                var entity = Entities[index];
                var inventoryItems = InventoryItemsLookup[entity];

                uint sum = 0;

                foreach (var item in inventoryItems)
                {
                    sum += item.Count;
                }
                
                ecb.SetComponent(index, entity, new Inventory.State{ ContainedItems = sum });
            }
        }
    }
}