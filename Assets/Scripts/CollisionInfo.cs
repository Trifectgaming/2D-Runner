using System;

namespace Assets.Scripts
{
    [Serializable]
    public struct CollisionInfo
    {
        public static readonly CollisionInfo Empty = new CollisionInfo();
        public bool Above;
        public bool Below;
        public bool Left;
        public bool Right;
    }
}