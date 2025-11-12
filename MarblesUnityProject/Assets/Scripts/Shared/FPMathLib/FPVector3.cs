using System;
using System.Runtime.CompilerServices;
using MemoryPack;
using Newtonsoft.Json;

namespace FPMathLib
{
    /// <summary>
    /// Deterministic fixed-point 3D vector
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    [MemoryPackable]
    public partial struct FPVector3 : IEquatable<FPVector3>
    {
        [JsonProperty("x")]
        public FP X;
        [JsonProperty("y")]
        public FP Y;
        [JsonProperty("z")]
        public FP Z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FPVector3(FP x, FP y, FP z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        // Constants
        public static readonly FPVector3 Zero = new FPVector3(FP.Zero, FP.Zero, FP.Zero);
        public static readonly FPVector3 One = new FPVector3(FP.One, FP.One, FP.One);
        public static readonly FPVector3 Right = new FPVector3(FP.One, FP.Zero, FP.Zero);
        public static readonly FPVector3 Left = new FPVector3(FP.MinusOne, FP.Zero, FP.Zero);
        public static readonly FPVector3 Up = new FPVector3(FP.Zero, FP.One, FP.Zero);
        public static readonly FPVector3 Down = new FPVector3(FP.Zero, FP.MinusOne, FP.Zero);
        public static readonly FPVector3 Forward = new FPVector3(FP.Zero, FP.Zero, FP.One);
        public static readonly FPVector3 Back = new FPVector3(FP.Zero, FP.Zero, FP.MinusOne);

        // Properties
        public FP Magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => FPMath.Sqrt(X * X + Y * Y + Z * Z);
        }

        public FP SqrMagnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => X * X + Y * Y + Z * Z;
        }

        public FPVector3 Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FP mag = Magnitude;
                if (mag > FP.Epsilon)
                    return new FPVector3(X / mag, Y / mag, Z / mag);
                return Zero;
            }
        }

        // Static methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Dot(FPVector3 a, FPVector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 Cross(FPVector3 a, FPVector3 b)
        {
            return new FPVector3(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Distance(FPVector3 a, FPVector3 b)
        {
            return (a - b).Magnitude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP SqrDistance(FPVector3 a, FPVector3 b)
        {
            return (a - b).SqrMagnitude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 Min(FPVector3 a, FPVector3 b)
        {
            return new FPVector3(
                FPMath.Min(a.X, b.X),
                FPMath.Min(a.Y, b.Y),
                FPMath.Min(a.Z, b.Z)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 Max(FPVector3 a, FPVector3 b)
        {
            return new FPVector3(
                FPMath.Max(a.X, b.X),
                FPMath.Max(a.Y, b.Y),
                FPMath.Max(a.Z, b.Z)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 Lerp(FPVector3 a, FPVector3 b, FP t)
        {
            t = FPMath.Clamp(t, FP.Zero, FP.One);
            return a + (b - a) * t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 Project(FPVector3 vector, FPVector3 onNormal)
        {
            FP sqrMag = Dot(onNormal, onNormal);
            if (sqrMag < FP.Epsilon)
                return Zero;
            FP dot = Dot(vector, onNormal);
            return onNormal * (dot / sqrMag);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 ProjectOnPlane(FPVector3 vector, FPVector3 planeNormal)
        {
            return vector - Project(vector, planeNormal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Angle(FPVector3 from, FPVector3 to)
        {
            FP denominator = FPMath.Sqrt(from.SqrMagnitude * to.SqrMagnitude);
            if (denominator < FP.Epsilon)
                return FP.Zero;

            FP dot = FPMath.Clamp(Dot(from, to) / denominator, FP.MinusOne, FP.One);
            return FPMath.Acos(dot);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 Scale(FPVector3 a, FPVector3 b)
        {
            return new FPVector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 Reflect(FPVector3 inDirection, FPVector3 inNormal)
        {
            return inDirection - FP.Two * Dot(inDirection, inNormal) * inNormal;
        }

        // Operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 operator +(FPVector3 a, FPVector3 b)
        {
            return new FPVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 operator -(FPVector3 a, FPVector3 b)
        {
            return new FPVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 operator -(FPVector3 a)
        {
            return new FPVector3(-a.X, -a.Y, -a.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 operator *(FPVector3 a, FP scalar)
        {
            return new FPVector3(a.X * scalar, a.Y * scalar, a.Z * scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 operator *(FP scalar, FPVector3 a)
        {
            return new FPVector3(a.X * scalar, a.Y * scalar, a.Z * scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 operator /(FPVector3 a, FP scalar)
        {
            return new FPVector3(a.X / scalar, a.Y / scalar, a.Z / scalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FPVector3 a, FPVector3 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FPVector3 a, FPVector3 b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }

        // Interface implementations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(FPVector3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is FPVector3 vector && Equals(vector);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X.GetHashCode(), Y.GetHashCode(), Z.GetHashCode());
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 FromFloats(float x, float y, float z)
        {
            return new FPVector3(FP.FromFloat(x), FP.FromFloat(y), FP.FromFloat(z));
        }
    }
}

