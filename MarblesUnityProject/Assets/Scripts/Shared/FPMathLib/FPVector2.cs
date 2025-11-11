using System;
using System.Runtime.CompilerServices;

namespace FPMathLib
{
    /// <summary>
    /// Deterministic fixed-point 2D vector
    /// </summary>
    [Serializable]
    public struct FPVector2 : IEquatable<FPVector2>
    {
        public FP X;
        public FP Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FPVector2(FP x, FP y)
        {
            X = x;
            Y = y;
        }

        // Constants
        public static readonly FPVector2 Zero = new FPVector2(FP.Zero, FP.Zero);
        public static readonly FPVector2 One = new FPVector2(FP.One, FP.One);
        public static readonly FPVector2 Right = new FPVector2(FP.One, FP.Zero);
        public static readonly FPVector2 Left = new FPVector2(FP.MinusOne, FP.Zero);
        public static readonly FPVector2 Up = new FPVector2(FP.Zero, FP.One);
        public static readonly FPVector2 Down = new FPVector2(FP.Zero, FP.MinusOne);

        // Properties
        public FP Magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => FPMath.Sqrt(X * X + Y * Y);
        }

        public FP SqrMagnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => X * X + Y * Y;
        }

        public FPVector2 Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FP mag = Magnitude;
                if (mag > FP.Epsilon)
                    return new FPVector2(X / mag, Y / mag);
                return Zero;
            }
        }

        // Static methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Dot(FPVector2 a, FPVector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Cross(FPVector2 a, FPVector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Distance(FPVector2 a, FPVector2 b)
        {
            return (a - b).Magnitude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP SqrDistance(FPVector2 a, FPVector2 b)
        {
            return (a - b).SqrMagnitude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 Min(FPVector2 a, FPVector2 b)
        {
            return new FPVector2(FPMath.Min(a.X, b.X), FPMath.Min(a.Y, b.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 Max(FPVector2 a, FPVector2 b)
        {
            return new FPVector2(FPMath.Max(a.X, b.X), FPMath.Max(a.Y, b.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 Lerp(FPVector2 a, FPVector2 b, FP t)
        {
            t = FPMath.Clamp(t, FP.Zero, FP.One);
            return a + (b - a) * t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 Rotate(FPVector2 v, FP angle)
        {
            FP cos = FPMath.Cos(angle);
            FP sin = FPMath.Sin(angle);
            return new FPVector2(
                v.X * cos - v.Y * sin,
                v.X * sin + v.Y * cos
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 Perpendicular(FPVector2 v)
        {
            return new FPVector2(-v.Y, v.X);
        }

        // Operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 operator +(FPVector2 a, FPVector2 b)
        {
            return new FPVector2(a.X + b.X, a.Y + b.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 operator -(FPVector2 a, FPVector2 b)
        {
            return new FPVector2(a.X - b.X, a.Y - b.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 operator -(FPVector2 a)
        {
            return new FPVector2(-a.X, -a.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 operator *(FPVector2 a, FP scalar)
        {
            return new FPVector2(a.X * scalar, a.Y * scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 operator *(FP scalar, FPVector2 a)
        {
            return new FPVector2(a.X * scalar, a.Y * scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 operator /(FPVector2 a, FP scalar)
        {
            return new FPVector2(a.X / scalar, a.Y / scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FPVector2 a, FPVector2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FPVector2 a, FPVector2 b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        // Interface implementations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(FPVector2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is FPVector2 vector && Equals(vector);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X.GetHashCode(), Y.GetHashCode());
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector2 FromFloats(float x, float y)
        {
            return new FPVector2(FP.FromFloat(x), FP.FromFloat(y));
        }
    }
}

