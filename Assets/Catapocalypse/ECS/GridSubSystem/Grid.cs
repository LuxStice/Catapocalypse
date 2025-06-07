using Unity.Entities;

namespace Catapocalypse.ECS.GridSubSystem
{
    public struct Grid : IComponentData
    {
        public static ComponentTypeSet TypeSet = new (new[]
        {
            ComponentType.ReadOnly<Grid>(), ComponentType.ReadOnly<GridData>(), ComponentType.ReadOnly<CellReference>(), 
        });
    }
}