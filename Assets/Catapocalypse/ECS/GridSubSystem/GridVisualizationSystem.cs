using Catapocalypse.ECS.PrefabSystem;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Catapocalypse.ECS.GridSubSystem
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class GridVisualizationSystem : SystemBase
    {
        private EntityQuery query;
        protected override void OnCreate()
        {
            base.OnCreate();
            query = GetEntityQuery(new[]
            {
                ComponentType.ReadOnly<MaterialColorData>(),
                ComponentType.ReadOnly<SharedGridReference>(),
            });
        }

        protected override void OnUpdate()
        {
            var job = new UpdateVisualizerValueJob
            {
                EntityTypeHandle = SystemAPI.GetEntityTypeHandle(),
                MaterialColorTypeHandle = SystemAPI.GetComponentTypeHandle<MaterialColorData>(true),
                CellDataTypeHandle = SystemAPI.GetComponentTypeHandle<GridCellData>(true),
                SharedGridTypeHandle = SystemAPI.GetSharedComponentTypeHandle<SharedGridReference>(),
                TotalTime = (float)SystemAPI.Time.ElapsedTime,
                ECB = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(World.Unmanaged).AsParallelWriter()
            }.ScheduleParallel(query, Dependency);
            
            Dependency = JobHandle.CombineDependencies(Dependency, job);
        }

        public struct UpdateVisualizerValueJob : IJobChunk
        {
            public EntityTypeHandle EntityTypeHandle;
            [ReadOnly] public ComponentTypeHandle<MaterialColorData> MaterialColorTypeHandle;
            [ReadOnly] public ComponentTypeHandle<GridCellData> CellDataTypeHandle;
            public SharedComponentTypeHandle<SharedGridReference> SharedGridTypeHandle;

            public float TotalTime;
            private const float frequency = 0.3f;

            public EntityCommandBuffer.ParallelWriter ECB;
            
            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
                in v128 chunkEnabledMask)
            {
                var sharedGridData = chunk.GetSharedComponent(SharedGridTypeHandle);
                
                var entities = chunk.GetNativeArray(EntityTypeHandle);
                var cellData = chunk.GetNativeArray(ref CellDataTypeHandle);
                var materialColors = chunk.GetNativeArray(ref MaterialColorTypeHandle);
                
                var enumerator =
                    new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.Count);
                
                Debug.Log($"Chunk has {entities.Length} entities");
                
                while (enumerator.NextEntityIndex(out var i))
                {
                    var cellEntity = entities[i];
                    var materialColor = materialColors[i];
                    int2 position = cellData[i].GridPosition;
                    
                    float raw = math.sin(TotalTime + position.x * frequency);
                    float normalized = (raw + 1f) * 0.5f;

                    materialColor.Value = new float4(normalized);
                    
                    ECB.SetComponent(unfilteredChunkIndex, cellEntity, materialColor);
                }
            }
        }
    }
}