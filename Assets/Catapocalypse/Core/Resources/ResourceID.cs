using System;

namespace Catapocalypse.Core.Resources
{
    [Serializable]
    public readonly struct ResourceID : IEquatable<ResourceID>, IComparable<ResourceID>, IComparable
    {
        public readonly uint InternalID;

        public ResourceID(uint internalID)
        {
            InternalID = internalID;
        }

        public override int GetHashCode() => InternalID.GetHashCode();

        public override string ToString() => $"ResourceID[{InternalID:00}]";

        public bool Equals(ResourceID other) => InternalID == other.InternalID;

        public override bool Equals(object obj)
        {
            return obj is ResourceID other && Equals(other);
        }

        public int CompareTo(ResourceID other)
        {
            return InternalID.CompareTo(other.InternalID);
        }

        public int CompareTo(object obj)
        {
            if (obj is null) return 1;
            return obj is ResourceID other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(ResourceID)}");
        }

        public static bool operator <(ResourceID left, ResourceID right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(ResourceID left, ResourceID right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(ResourceID left, ResourceID right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(ResourceID left, ResourceID right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}