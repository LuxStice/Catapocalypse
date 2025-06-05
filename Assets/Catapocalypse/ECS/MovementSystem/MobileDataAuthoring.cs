using Unity.Entities;
using UnityEngine;

namespace Catapocalypse.ECS.MovementSystem
{
    [RequireComponent(typeof(PathSeekerAuthoring))]
    public class MobileAuthoring : MonoBehaviour
    {
        [Range(0.0f, 20)]
        public double Speed = 5;

        public class MobileBaker : Baker<MobileAuthoring>
        {
            public override void Bake(MobileAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new MobileData { Speed = authoring.Speed });
                AddComponent<MobileData.Velocity>(entity);
            }
        }
    }
}