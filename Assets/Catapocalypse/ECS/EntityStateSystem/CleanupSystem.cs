using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Catapocalypse.ECS.EntityStateSystem
{
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup), OrderLast = true)]
    public partial class CleanupSystem : SystemBase
    {
        private ComponentTypeSet updatedTypes;

        private Entity entity;
        
        protected override void OnCreate()
        {
            updatedTypes = new ComponentTypeSet(ComponentType.ReadOnly<Created>(), ComponentType.ReadOnly<Updated>());
            entity = EntityManager.CreateSingleton<Singleton>("Cleanup System Singleton");
            
            RequireForUpdate<Singleton>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            var data = SystemAPI.GetSingleton<Singleton>();
            
            data.DeletedHandle.Complete();
            data.UpdatedHandle.Complete();

            if (data.DeletedEntities is { IsCreated: true, IsEmpty: false })
            {
                EntityManager.DestroyEntity(data.DeletedEntities.ToArray(Allocator.TempJob));
            }
            data.DeletedEntities.Dispose();

            if (data.UpdatedEntities is { IsCreated: true, IsEmpty: false })
            {
                EntityManager.RemoveComponent(data.UpdatedEntities.ToArray(Allocator.TempJob), in updatedTypes);
            }
            data.UpdatedEntities.Dispose();

            EntityManager.SetComponentData(entity, new Singleton());
        }

        protected override void OnDestroy()
        {
        }

        public struct Singleton : IComponentData
        {
            public NativeList<Entity> DeletedEntities;
            public NativeList<Entity> UpdatedEntities;
            public JobHandle DeletedHandle;
            public JobHandle UpdatedHandle;
        }
    }
}