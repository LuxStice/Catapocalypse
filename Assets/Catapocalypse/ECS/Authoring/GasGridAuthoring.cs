using Catapocalypse.ECS.GridSubSystem;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Catapocalypse.ECS.Authoring
{
    [RequireComponent(typeof(GridAuthoring))]
    public class GasGridAuthoring : MonoBehaviour
    {
        public int2 Size => grid.Size;
        public float CellSize => grid.CellSize;
        public GridAuthoring grid;
        public class DataBaker : Baker<GasGridAuthoring>
        {
            public override void Bake(GasGridAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, GasGrid.TypeSet);
            }
        }

        private void OnValidate()
        {
            grid = GetComponent<GridAuthoring>();
        }
    }
}