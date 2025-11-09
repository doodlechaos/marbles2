using System;

namespace LockSim
{
    public enum ShapeType : byte
    {
        Box = 0,
        Circle = 1
    }

    [Serializable]
    public struct BoxShape
    {
        public FP HalfWidth;
        public FP HalfHeight;

        public BoxShape(FP halfWidth, FP halfHeight)
        {
            HalfWidth = halfWidth;
            HalfHeight = halfHeight;
        }

        public static BoxShape FromSize(FP width, FP height)
        {
            return new BoxShape(width * FP.Half, height * FP.Half);
        }
    }

    [Serializable]
    public struct CircleShape
    {
        public FP Radius;

        public CircleShape(FP radius)
        {
            Radius = radius;
        }
    }

    [Serializable]
    public struct AABB
    {
        public FPVector2 Min;
        public FPVector2 Max;

        public AABB(FPVector2 min, FPVector2 max)
        {
            Min = min;
            Max = max;
        }

        public FPVector2 Center => (Min + Max) * FP.Half;
        public FPVector2 Extents => (Max - Min) * FP.Half;

        public bool Overlaps(AABB other)
        {
            return !(Max.X < other.Min.X || Min.X > other.Max.X ||
                     Max.Y < other.Min.Y || Min.Y > other.Max.Y);
        }

        public static AABB Combine(AABB a, AABB b)
        {
            return new AABB(
                FPVector2.Min(a.Min, b.Min),
                FPVector2.Max(a.Max, b.Max)
            );
        }
    }
}

