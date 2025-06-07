using Unity.Entities;
using Unity.Mathematics;

namespace Catapocalypse.ECS.GridSubSystem
{
    public struct CellReference : IBufferElementData
    {
        public int Index;
        public int2 Position;
        public Entity Entity;
    }
}