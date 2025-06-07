using Catapocalypse.ECS.InventorySystem;
using Unity.Entities;
using UnityEngine;

namespace Catapocalypse.ECS.GathererSystem
{
    [RequireComponent(typeof(InventoryAuthoring))]
    public class GathererAuthoring : MonoBehaviour
    {
        public double Rate;

        public class GathererBaker : Baker<GathererAuthoring>
        {
            public override void Bake(GathererAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Gatherer { Rate = authoring.Rate });
                AddComponent(entity, new Gatherer.StateMachine());
                AddComponent(entity, new Gatherer.StateMachine.Blackboard());
                AddComponent(entity, new Gatherer.StateMachine.State());
            }
        }
    }
}