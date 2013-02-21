using System;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public struct SpeedInfo : IEquatable<Vector3>
    {
        public static readonly SpeedInfo Empty = new SpeedInfo();
        public Null<float> minX;
        public Null<float> minY;
        public Null<float> maxX;
        public Null<float> maxY;
        
        public bool Equals(Vector3 other)
        {
            bool isValid = true;
            if (minX.HasValue)
            {
                isValid = minX.Value >= other.x;
            }
            if (isValid && minY.HasValue)
            {
                isValid = minY.Value >= other.y;
            }
            if (isValid && maxX.HasValue)
            {
                isValid = maxX.Value <= other.x;
            }
            if (isValid && maxY.HasValue)
            {
                isValid = maxY.Value <= other.y;
            }
            return isValid;
        }
    }

    [Serializable]
    public struct Null<T> where T : struct
    {
        public T Value;
        public bool HasValue;
        public static implicit operator Null<T>(T value)
        {
            return new Null<T>
                       {
                           HasValue = true,
                           Value = value,
                       };
        }
    }
}