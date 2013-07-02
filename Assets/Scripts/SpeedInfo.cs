using System;
using UnityEngine;

[Serializable]
public struct SpeedInfo : IEquatable<Vector3>
{
    public static readonly SpeedInfo Empty = new SpeedInfo();
    public float? minX;
    public float? minY;
    public float? maxX;
    public float? maxY;

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
