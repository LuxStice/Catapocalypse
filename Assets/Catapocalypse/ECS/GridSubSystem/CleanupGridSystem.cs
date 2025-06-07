using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Catapocalypse.ECS.GridSubSystem
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(SpawnGridSystem))]
    public partial struct CleanupGridSystem : ISystem
    {
        private EntityQuery toClearQuery;
        private Entity entity;

        private BufferLookup<CellReference> cellReferenceComponentLookup;

        public void OnCreate(ref SystemState state)
        {
            toClearQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Grid, GridData, CellReference>()
                .Build(ref state);
            toClearQuery.SetChangedVersionFilter(ComponentType.ReadOnly<GridData>());

            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
            cellReferenceComponentLookup = SystemAPI.GetBufferLookup<CellReference>();
            state.RequireForUpdate(toClearQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            cellReferenceComponentLookup.Update(ref state);

            var toClearEntities = toClearQuery.ToEntityArray(Allocator.TempJob);
            var job1 = new ClearGridJob
            {
                Entities = toClearEntities.AsReadOnly(),
                CellReferenceBufferLookup = cellReferenceComponentLookup,
                ECB = ecb.AsParallelWriter()
            }.Schedule(toClearEntities.Length, 32, state.Dependency);
            toClearEntities.Dispose(job1);
            state.Dependency = JobHandle.CombineDependencies(state.Dependency, job1);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        public struct ClearGridJob : IJobParallelFor
        {
            public NativeArray<Entity>.ReadOnly Entities;
            [ReadOnly] public BufferLookup<CellReference> CellReferenceBufferLookup;
            public EntityCommandBuffer.ParallelWriter ECB;

            public void Execute(int index)
            {
                var entity = Entities[index];
                var buffer = CellReferenceBufferLookup[entity];
                foreach (var cellReference in buffer)
                {
                    var cellEntity = cellReference.Entity;
                    ECB.DestroyEntity(index, cellEntity);
                }

                var bufferWO = ECB.SetBuffer<CellReference>(index, entity);
                bufferWO.Clear();
            }
        }
    }
}