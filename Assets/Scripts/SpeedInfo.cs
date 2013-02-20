using System;

namespace Assets.Scripts
{
    [Serializable]
    public struct SpeedInfo
    {
        public static readonly SpeedInfo Empty = new SpeedInfo();
        public float? minX;
        public float? minY;
        public float? maxX;
        public float? maxY;
    }
}