using Unity.Entities;
using Unity.Mathematics;

namespace Catapocalypse.ECS.GridSubSystem
{
    public struct GridCellData : IComponentData
    {
        public int2 GridPosition;
        
        public static ComponentTypeSet TypeSet = new (new[]
        {
            ComponentType.ReadOnly<GridCellData>(), ComponentType.ReadOnly<GridCellVisualizerData>(),
            ComponentType.ReadOnly<SharedGridReference>(), ComponentType.ReadOnly<MaterialColorData>(), 
        });
    }
}