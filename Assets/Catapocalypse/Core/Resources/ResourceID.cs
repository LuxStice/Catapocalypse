using System;
using Unity.Burst;
using UnityEngine;

namespace Catapocalypse.Core.Resources
{
    [Serializable]
    [BurstCompile]
    public struct ResourceID : IEquatable<ResourceID>, IComparable<ResourceID>, IComparable
    {
        public uint InternalID;

        public ResourceID(uint internalID)
        {
            InternalID = internalID;
        }

        [BurstCompile]
        public override int GetHashCode() => InternalID.GetHashCode();

        public override string ToString() => $"ResourceID[{InternalID:00}]";

        [BurstCompile]
        public bool Equals(ResourceID other) => InternalID == other.InternalID;

        public override bool Equals(object obj)
        {
            return obj is ResourceID other && Equals(other);
        }

        [BurstCompile]
        public int CompareTo(ResourceID other)
        {
            return InternalID.CompareTo(other.InternalID);
        }

        public int CompareTo(object obj)
        {
            if (obj is null) return 1;
            return obj is ResourceID other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(ResourceID)}");
        }

        [BurstCompile]
        public static bool operator ==(ResourceID left, ResourceID right) => left.InternalID == right.InternalID;
        [BurstCompile]
        public static bool operator !=(ResourceID left, ResourceID right) => left.InternalID != right.InternalID;
        
        [BurstCompile]
        public static bool operator <(ResourceID left, ResourceID right)
        {
            return left.CompareTo(right) < 0;
        }

        [BurstCompile]
        public static bool operator >(ResourceID left, ResourceID right)
        {
            return left.CompareTo(right) > 0;
        }

        [BurstCompile]
        public static bool operator <=(ResourceID left, ResourceID right)
        {
            return left.CompareTo(right) <= 0;
        }

        [BurstCompile]
        public static bool operator >=(ResourceID left, ResourceID right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}