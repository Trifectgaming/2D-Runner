using System;

namespace Assets.Scripts
{
    [Serializable]
    public class CollisionInfo
    {
        public static readonly CollisionInfo Empty = new CollisionInfo();

        public CollisionInfo()
        {
            Above = new Null<bool>();
            Below = new Null<bool>();
            Left = new Null<bool>();
            Above = new Null<bool>();
        }

        public Null<bool> Above;
        public Null<bool> Below;
        public Null<bool> Left;
        public Null<bool> Right;

        public bool Valid(CollisionInfo other)
        {
            var equals =
                (!Above.HasValue || Above.Equals(other.Above)) &&
                (!Below.HasValue || Below.Equals(other.Below)) &&
                (!Left.HasValue || Left.Equals(other.Left)) &&
                (!Right.HasValue || Right.Equals(other.Right));
            return equals;
        }

        public override string ToString()
        {
            return string.Format("CI [{0},{1},{2},{3}]",
                Above,
                Right,
                Below,
                Left
                );
        }
    }
}