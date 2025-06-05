using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Catapocalypse.ECS.EntityStateSystem
{
    /// <summary>
    /// This system gets all entities that have been either updated or deleted on the last frame and adds them to the
    /// cleanup system, executed as the last system of the frame. Meaning that the cleanup only happens on the next frame.
    /// Giving the opportunity for every system to deal with the entity before the tag is removed.
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    [BurstCompile]
    public partial class PrepareCleanupSystem : SystemBase
    {
        private EntityQuery deletedQuery;
        private EntityQuery updatedQuery;

        protected override void OnCreate()
        {
            deletedQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAny<Deleted>()
                .Build(this);
        
            updatedQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAny<Created, Updated>()
                .Build(this);
            
            RequireForUpdate<CleanupSystem.Singleton>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            var cleanupEntity = SystemAPI.GetSingletonEntity<CleanupSystem.Singleton>();
            
            var deletedList = deletedQuery.ToEntityListAsync(Allocator.TempJob, out var deletedHandle);
            var updatedList = updatedQuery.ToEntityListAsync(Allocator.TempJob, out var updatedHandle);
            
            this.Dependency = JobHandle.CombineDependencies(Dependency, deletedHandle, updatedHandle);

            EntityManager.AddComponentData(cleanupEntity, new CleanupSystem.Singleton
            {
                DeletedEntities = deletedList,
                UpdatedEntities = updatedList,
                DeletedHandle = deletedHandle,
                UpdatedHandle = updatedHandle
            });
        }
    }
}