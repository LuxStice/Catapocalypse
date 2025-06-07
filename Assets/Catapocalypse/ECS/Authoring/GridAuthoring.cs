using Catapocalypse.ECS.GridSubSystem;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Catapocalypse.ECS.Authoring
{
    public class GridAuthoring : MonoBehaviour
    {
        public int2 Size = new(1024, 1024);
        public int Width => Size.x;
        public int Height => Size.y;
        public float CellSize = 1;

        public class Baker : Baker<GridAuthoring>
        {
            public override void Bake(GridAuthoring authoring)
            {
                var gridEntity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(gridEntity, new GridSubSystem.Grid { });
                AddComponent(gridEntity, new GridData { Size = authoring.Size, CellSize = authoring.CellSize });
                AddBuffer<CellReference>(gridEntity);
            }
        }

        private void OnDrawGizmosSelected()
        {
            var halfSize = CellSize / 2f;
            for (float y = (-Size.y/2f * CellSize) + halfSize; y <= Size.y/2f * CellSize; y+=CellSize)
            {
                for (float x = (-Size.x/2f * CellSize) + halfSize; x <= Size.x/2f * CellSize; x+=CellSize)
                {
                    var center = new Vector3(x, 0, y);
                    
                    Gizmos.DrawWireCube(center, new Vector3(1,0,1) * CellSize);
                }
            }
        }
    }
}