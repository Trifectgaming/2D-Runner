using System;

[Serializable]
public class CollisionInfo
{
    public static readonly CollisionInfo Empty = new CollisionInfo();

    public CollisionInfo()
    {
        Above = new bool?();
        Below = new bool?();
        Left = new bool?();
        Above = new bool?();
    }

    public bool? Above;
    public bool? Below;
    public bool? Left;
    public bool? Right;

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