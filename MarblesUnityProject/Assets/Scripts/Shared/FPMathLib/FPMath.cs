using System;
using System.Runtime.CompilerServices;

namespace FPMathLib
{
    /// <summary>
    /// Deterministic fixed-point math functions
    /// </summary>
    public static class FPMath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Abs(FP value)
        {
            return value.RawValue < 0 ? new FP(-value.RawValue) : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Min(FP a, FP b)
        {
            return a.RawValue < b.RawValue ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Max(FP a, FP b)
        {
            return a.RawValue > b.RawValue ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Clamp(FP value, FP min, FP max)
        {
            if (value.RawValue < min.RawValue)
                return min;
            if (value.RawValue > max.RawValue)
                return max;
            return value;
        }

        public static FP Sqrt(FP value)
        {
            if (value.RawValue < 0)
                return FP.Zero;

            if (value.RawValue == 0)
                return FP.Zero;

            // Newton-Raphson method for fixed-point square root
            long x = value.RawValue;
            long guess = x >> 1;
            if (guess == 0)
                guess = 1;

            for (int i = 0; i < 10; i++)
            {
                long nextGuess = (guess + (x << FP.SHIFT) / guess) >> 1;
                if (Abs(FP.FromRaw(nextGuess - guess)).RawValue < 10)
                    break;
                guess = nextGuess;
            }

            return FP.FromRaw(guess);
        }

        // Sine lookup table for angles 0-90 degrees (in 256ths of a full circle)
        // Values are in Q16.16 fixed-point format (multiply by 65536 for 1.0)
        private static readonly long[] SinLookup = new long[]
        {
            0, 1608, 3216, 4821, 6424, 8022, 9616, 11204, 12785, 14359,
            15924, 17479, 19024, 20557, 22078, 23586, 25080, 26558, 28020, 29465,
            30893, 32303, 33692, 35062, 36410, 37736, 39040, 40320, 41576, 42806,
            44011, 45190, 46341, 47464, 48559, 49624, 50660, 51665, 52639, 53581,
            54491, 55368, 56212, 57022, 57798, 58538, 59244, 59914, 60547, 61145,
            61705, 62228, 62714, 63162, 63572, 63943, 64277, 64571, 64827, 65043,
            65220, 65358, 65457, 65516, 65536
        };

        public static FP Sin(FP angle)
        {
            // Normalize angle to 0-2PI range
            FP twoPi = FP.Pi * FP.Two;
            while (angle.RawValue < 0)
                angle = angle + twoPi;
            while (angle >= twoPi)
                angle = angle - twoPi;

            // Convert to lookup table position (0-256 for full circle)
            // Keep fractional part for interpolation
            FP normalized = angle * FP.FromInt(256) / twoPi;
            int index = normalized.ToInt();
            FP fraction = normalized - FP.FromInt(index);

            // Determine quadrant and get interpolated value
            if (index < 64)
            {
                return InterpolateSin(index, fraction, false);
            }
            else if (index < 128)
            {
                // Mirror around 64: indices 64-127 map to lookup indices 64-0
                int mirroredIndex = 127 - index;
                // Fraction is inverted when mirroring
                return InterpolateSin(mirroredIndex, FP.One - fraction, false);
            }
            else if (index < 192)
            {
                // Third quadrant: negative of first quadrant
                int offsetIndex = index - 128;
                return InterpolateSin(offsetIndex, fraction, true);
            }
            else
            {
                // Fourth quadrant: negative, mirrored
                int mirroredIndex = 255 - index;
                return InterpolateSin(mirroredIndex, FP.One - fraction, true);
            }
        }

        /// <summary>
        /// Interpolates between lookup table entries for better precision with small angles.
        /// </summary>
        private static FP InterpolateSin(int index, FP fraction, bool negate)
        {
            // Clamp index to valid range
            if (index < 0) index = 0;
            if (index >= SinLookup.Length - 1)
            {
                long result = SinLookup[SinLookup.Length - 1];
                return FP.FromRaw(negate ? -result : result);
            }

            // Linear interpolation between lookup[index] and lookup[index+1]
            long v0 = SinLookup[index];
            long v1 = SinLookup[index + 1];

            // Interpolate: result = v0 + (v1 - v0) * fraction
            long diff = v1 - v0;
            long interpolated = v0 + ((diff * fraction.RawValue) >> FP.SHIFT);

            return FP.FromRaw(negate ? -interpolated : interpolated);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Cos(FP angle)
        {
            return Sin(angle + FP.PiOver2);
        }

        public static FP Atan2(FP y, FP x)
        {
            // Simple approximation for atan2
            if (x == FP.Zero)
            {
                if (y > FP.Zero)
                    return FP.PiOver2;
                if (y < FP.Zero)
                    return -FP.PiOver2;
                return FP.Zero;
            }

            FP z = y / x;
            FP absZ = Abs(z);

            FP angle;
            if (absZ < FP.One)
            {
                angle = z / (FP.One + FP.FromFloat(0.28f) * z * z);
            }
            else
            {
                angle = FP.PiOver2 - z / (z * z + FP.FromFloat(0.28f));
            }

            if (x < FP.Zero)
            {
                if (y < FP.Zero)
                    return angle - FP.Pi;
                else
                    return angle + FP.Pi;
            }

            return angle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Sign(FP value)
        {
            if (value.RawValue > 0)
                return FP.One;
            if (value.RawValue < 0)
                return FP.MinusOne;
            return FP.Zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Floor(FP value)
        {
            return FP.FromRaw(value.RawValue & ~((1L << FP.SHIFT) - 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Ceil(FP value)
        {
            long fractional = value.RawValue & ((1L << FP.SHIFT) - 1);
            if (fractional == 0)
                return value;
            return FP.FromRaw((value.RawValue & ~((1L << FP.SHIFT) - 1)) + (1L << FP.SHIFT));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Round(FP value)
        {
            return Floor(value + FP.Half);
        }

        public static FP Acos(FP value)
        {
            // Clamp input to valid range [-1, 1]
            if (value <= FP.MinusOne)
                return FP.Pi;
            if (value >= FP.One)
                return FP.Zero;

            // Use approximation: acos(x) ≈ sqrt(1-x) * (a0 + a1*x + a2*x^2 + a3*x^3)
            // This is a polynomial approximation for acos
            bool negative = value.RawValue < 0;
            FP x = Abs(value);

            FP ret = FP.FromFloat(-0.0187293f);
            ret = ret * x;
            ret = ret + FP.FromFloat(0.0742610f);
            ret = ret * x;
            ret = ret - FP.FromFloat(0.2121144f);
            ret = ret * x;
            ret = ret + FP.FromFloat(1.5707288f);
            ret = ret * Sqrt(FP.One - x);

            return negative ? FP.Pi - ret : ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Asin(FP value)
        {
            // asin(x) = π/2 - acos(x)
            return FP.PiOver2 - Acos(value);
        }
    }
}

