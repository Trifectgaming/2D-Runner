using System;

namespace Assets.Scripts
{
    [Serializable]
    public class CollisionInfo : IEquatable<CollisionInfo>
    {
        public bool Equals(CollisionInfo other)
        {
            return Above.Equals(other.Above) && Below.Equals(other.Below) && Left.Equals(other.Left) && Right.Equals(other.Right);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Above.GetHashCode();
                hashCode = (hashCode*397) ^ Below.GetHashCode();
                hashCode = (hashCode*397) ^ Left.GetHashCode();
                hashCode = (hashCode*397) ^ Right.GetHashCode();
                return hashCode;
            }
        }

        public static readonly CollisionInfo Empty = new CollisionInfo();
        public bool Above;
        public bool Below;
        public bool Left;
        public bool Right;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is CollisionInfo && Equals((CollisionInfo) obj);
        }
    }
}