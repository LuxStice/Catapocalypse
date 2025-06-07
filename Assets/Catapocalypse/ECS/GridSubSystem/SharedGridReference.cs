using Unity.Entities;
using Unity.Mathematics;

namespace Catapocalypse.ECS.GridSubSystem
{
    public partial struct SharedGridReference : ISharedComponentData
    {
        public int2 Size;
        public float CellSize;

        public override int GetHashCode() => Size.GetHashCode() ^ CellSize.GetHashCode();
    }
}