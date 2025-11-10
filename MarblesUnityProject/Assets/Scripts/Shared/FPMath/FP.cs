using System;
using System.Runtime.CompilerServices;

namespace FPMath
{
    /// <summary>
    /// Deterministic fixed-point number using Q16.16 format (16 bits integer, 16 bits fractional)
    /// </summary>
    [Serializable]
    public struct FP : IEquatable<FP>, IComparable<FP>
    {
        public const int SHIFT = 16;
        public const int FRACTIONAL_BITS = 16;
        public const long ONE_RAW = 1L << SHIFT;
        
        public long RawValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FP(long rawValue)
        {
            RawValue = rawValue;
        }

        // Constants
        public static readonly FP Zero = new FP(0);
        public static readonly FP One = new FP(ONE_RAW);
        public static readonly FP Half = new FP(ONE_RAW / 2);
        public static readonly FP Two = new FP(ONE_RAW * 2);
        public static readonly FP MinusOne = new FP(-ONE_RAW);
        public static readonly FP MaxValue = new FP(long.MaxValue);
        public static readonly FP MinValue = new FP(long.MinValue);
        public static readonly FP Pi = new FP(205887L); // ~3.14159
        public static readonly FP PiOver2 = new FP(102944L); // ~1.5708
        public static readonly FP Epsilon = new FP(1L);

        // Conversions
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP FromInt(int value) => new FP((long)value << SHIFT);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP FromLong(long value) => new FP(value << SHIFT);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP FromFloat(float value) => new FP((long)(value * ONE_RAW));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP FromDouble(double value) => new FP((long)(value * ONE_RAW));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP FromRaw(long rawValue) => new FP(rawValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ToInt() => (int)(RawValue >> SHIFT);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ToFloat() => (float)RawValue / ONE_RAW;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ToDouble() => (double)RawValue / ONE_RAW;

        // Arithmetic operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator +(FP a, FP b) => new FP(a.RawValue + b.RawValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator -(FP a, FP b) => new FP(a.RawValue - b.RawValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator -(FP a) => new FP(-a.RawValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator *(FP a, FP b) => new FP((a.RawValue * b.RawValue) >> SHIFT);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator /(FP a, FP b) => new FP((a.RawValue << SHIFT) / b.RawValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP operator %(FP a, FP b) => new FP(a.RawValue % b.RawValue);

        // Comparison operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FP a, FP b) => a.RawValue == b.RawValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FP a, FP b) => a.RawValue != b.RawValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(FP a, FP b) => a.RawValue < b.RawValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(FP a, FP b) => a.RawValue > b.RawValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(FP a, FP b) => a.RawValue <= b.RawValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(FP a, FP b) => a.RawValue >= b.RawValue;

        // Interface implementations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(FP other) => RawValue == other.RawValue;

        public override bool Equals(object obj) => obj is FP fp && Equals(fp);

        public override int GetHashCode() => RawValue.GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(FP other) => RawValue.CompareTo(other.RawValue);

        public override string ToString() => ToDouble().ToString("F4");
    }
}

