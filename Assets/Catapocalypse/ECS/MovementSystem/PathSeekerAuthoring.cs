using Unity.Entities;
using UnityEngine;

namespace Catapocalypse.ECS.MovementSystem
{
    public class PathSeekerAuthoring : MonoBehaviour
    {
        public class PathBaker : Baker<PathSeekerAuthoring>
        {
            public override void Bake(PathSeekerAuthoring seekerAuthoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<PathSeeker>(entity);
                SetComponentEnabled<PathSeeker>(entity,false);
                
                AddComponent<PathSeeker.State>(entity);
                AddBuffer<Path.Node>(entity);
            }
        }
    }
}