using System;
using Catapocalypse.ECS.PrefabSystem;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Catapocalypse.ECS.GridSubSystem
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SpawnGridSystem : ISystem
    {
        private EntityQuery allQuery;
        private Entity entity;

        private BufferLookup<CellReference> cellReferenceComponentLookup;

        private Entity gridCellPrefab;

        public void OnCreate(ref SystemState state)
        {
            allQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Grid, GridData>()
                .Build(ref state);
            allQuery.SetChangedVersionFilter(ComponentType.ReadOnly<GridData>());

            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<PrefabDatabase>();
            state.RequireForUpdate(allQuery);
            
            cellReferenceComponentLookup = SystemAPI.GetBufferLookup<CellReference>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            cellReferenceComponentLookup.Update(ref state);

            if (gridCellPrefab == Entity.Null)
            {
                var prefabDatabase = SystemAPI.GetSingletonBuffer<PrefabReference>();

                if (!TryGetPrefab(prefabDatabase, "debug_visualizer", out gridCellPrefab))
                    throw new Exception($"Could not find prefab with key: debug_visualizer");
            }

            var gridEntities = allQuery.ToEntityArray(Allocator.TempJob);
            var gridDatas = allQuery.ToComponentDataArray<GridData>(Allocator.TempJob);

            var job = new SpawnGridJob
            {
                Entities = gridEntities.AsReadOnly(),
                GridDatas = gridDatas.AsReadOnly(),
                Prefab = gridCellPrefab,
                ECB = ecb.AsParallelWriter()
            }.Schedule(gridEntities.Length, 32, state.Dependency);
            gridEntities.Dispose(job);

            state.Dependency = JobHandle.CombineDependencies(state.Dependency, job);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
        

        private static bool TryGetPrefab(DynamicBuffer<PrefabReference> buffer, FixedString32Bytes key, out Entity prefab)
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

        public struct SpawnGridJob : IJobParallelFor
        {
            public NativeArray<Entity>.ReadOnly Entities;
            public NativeArray<GridData>.ReadOnly GridDatas;

            public EntityCommandBuffer.ParallelWriter ECB;
            public Entity Prefab;

            public void Execute(int index)
            {
                var gridEntity = Entities[index];
                var gridData = GridDatas[index];
                var buffer = ECB.SetBuffer<CellReference>(index, gridEntity);

                var sharedReference = new SharedGridReference
                {
                    Size = gridData.Size,
                    CellSize = gridData.CellSize
                };

                for (int y = 0; y < gridData.Height; y++)
                {
                    for (int x = 0; x < gridData.Width; x++)
                    {
                        var cellIndex = x + y * gridData.Height;
                        var cellEntity = ECB.Instantiate(index, Prefab);

                        ECB.SetComponent(index, cellEntity, new LocalTransform
                        {
                            Position = new float3((x - gridData.Width / 2f) * gridData.CellSize, 0,
                                (y - gridData.Height / 2f) * gridData.CellSize),
                            Scale = gridData.CellSize,
                            Rotation = quaternion.identity
                        });
                        ECB.AddComponent(index, cellEntity, new Parent { Value = gridEntity });
                        
                        ECB.SetComponent(index, cellEntity, new GridCellData() {  GridPosition = new int2(x,y) });
                        ECB.SetSharedComponent(index, cellEntity, sharedReference);

                        // Add cell to Grid
                        buffer.Add(new()
                        {
                            Index = cellIndex,
                            Position = new int2(x, y),
                            Entity = cellEntity
                        });
                    }
                }
            }
        }
    }
}