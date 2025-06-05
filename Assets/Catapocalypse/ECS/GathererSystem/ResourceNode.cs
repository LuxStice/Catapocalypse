using System;
using Catapocalypse.Core.Resources;
using Unity.Collections;
using Unity.Entities;

namespace Catapocalypse.ECS.GathererSystem
{
    public struct ResourceNode : IComponentData
    {
        public bool Depleted;
        [Serializable]
        public struct AvailableResource : IBufferElementData
        {
            public ResourceID ResourceID;
            public double Rate;
        }

        public struct State : IComponentData
        {
            public NativeHashMap<ResourceID, uint> ResourceNodes;
        }
    }
}