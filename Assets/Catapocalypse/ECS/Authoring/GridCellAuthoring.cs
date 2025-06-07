using Catapocalypse.ECS.GridSubSystem;
using Unity.Entities;
using UnityEngine;

namespace Catapocalypse.ECS.Authoring
{
    public class GridCellAuthoring : MonoBehaviour
    {
        private class GridCellAuthoringBaker : Baker<GridCellAuthoring>
        {
            public override void Bake(GridCellAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, GridCellData.TypeSet);
            }
        }
    }
}