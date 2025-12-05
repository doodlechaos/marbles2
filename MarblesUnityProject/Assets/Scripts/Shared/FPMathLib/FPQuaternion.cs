using System;
using System.Runtime.CompilerServices;
using MemoryPack;
using Newtonsoft.Json;

namespace FPMathLib
{
    /// <summary>
    /// Deterministic fixed-point quaternion for 3D rotations
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    [MemoryPackable]
    public partial struct FPQuaternion : IEquatable<FPQuaternion>
    {
        [JsonProperty("x")]
        public FP X;

        [JsonProperty("y")]
        public FP Y;

        [JsonProperty("z")]
        public FP Z;

        [JsonProperty("w")]
        public FP W;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FPQuaternion(FP x, FP y, FP z, FP w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        // Constants
        public static readonly FPQuaternion Identity = new FPQuaternion(
            FP.Zero,
            FP.Zero,
            FP.Zero,
            FP.One
        );

        // Properties
        public FP Magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => FPMath.Sqrt(X * X + Y * Y + Z * Z + W * W);
        }

        public FPQuaternion Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                FP mag = Magnitude;
                if (mag > FP.Epsilon)
                    return new FPQuaternion(X / mag, Y / mag, Z / mag, W / mag);
                return Identity;
            }
        }

        public FPQuaternion Conjugate
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new FPQuaternion(-X, -Y, -Z, W);
        }

        /// <summary>
        /// Converts this quaternion to Euler angles (in degrees).
        /// Returns angles in the order (X, Y, Z) using Unity's ZXY rotation order.
        /// ZXY means: apply Z rotation first, then X, then Y.
        /// </summary>
        public FPVector3 EulerAngles
        {
            get
            {
                FPVector3 euler;

                // X rotation (pitch - applied second in ZXY order)
                // sinX = 2*(w*x - y*z)
                FP sinX = FP.Two * (W * X - Y * Z);
                if (FPMath.Abs(sinX) >= FP.One)
                    euler.X = (sinX >= FP.Zero ? FP.FromInt(90) : FP.FromInt(-90)); // Gimbal lock at ±90°
                else
                    euler.X = FPMath.Asin(sinX) * FP.Rad2Deg;

                // Y rotation (yaw - applied last in ZXY order)
                // sinY = 2*(w*y + x*z), cosY = 1 - 2*(x² + y²)
                FP sinY = FP.Two * (W * Y + X * Z);
                FP cosY = FP.One - FP.Two * (X * X + Y * Y);
                euler.Y = FPMath.Atan2(sinY, cosY) * FP.Rad2Deg;

                // Z rotation (roll - applied first in ZXY order)
                // sinZ = 2*(w*z + x*y), cosZ = 1 - 2*(x² + z²)
                FP sinZ = FP.Two * (W * Z + X * Y);
                FP cosZ = FP.One - FP.Two * (X * X + Z * Z);
                euler.Z = FPMath.Atan2(sinZ, cosZ) * FP.Rad2Deg;

                return euler;
            }
        }

        // Static methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FP Dot(FPQuaternion a, FPQuaternion b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
        }

        /// <summary>
        /// Creates a quaternion from Euler angles specified in DEGREES.
        /// Rotation order: ZXY (Unity's default) - applies Z first, then X, then Y.
        /// This matches Unity's Quaternion.Euler() behavior.
        /// </summary>
        /// <param name="xDegrees">Rotation around X-axis in degrees</param>
        /// <param name="yDegrees">Rotation around Y-axis in degrees</param>
        /// <param name="zDegrees">Rotation around Z-axis in degrees</param>
        public static FPQuaternion Euler(FP xDegrees, FP yDegrees, FP zDegrees)
        {
            // Convert degrees to radians for trig functions
            FP xRad = xDegrees * FP.Deg2Rad;
            FP yRad = yDegrees * FP.Deg2Rad;
            FP zRad = zDegrees * FP.Deg2Rad;

            FP cx = FPMath.Cos(xRad * FP.Half);
            FP cy = FPMath.Cos(yRad * FP.Half);
            FP cz = FPMath.Cos(zRad * FP.Half);
            FP sx = FPMath.Sin(xRad * FP.Half);
            FP sy = FPMath.Sin(yRad * FP.Half);
            FP sz = FPMath.Sin(zRad * FP.Half);

            // ZXY rotation order: Q = Qy * Qx * Qz
            FPQuaternion q;
            q.W = cy * cx * cz + sy * sx * sz;
            q.X = cy * sx * cz + sy * cx * sz;
            q.Y = sy * cx * cz - cy * sx * sz;
            q.Z = cy * cx * sz - sy * sx * cz;

            return q;
        }

        /// <summary>
        /// Creates a quaternion from Euler angles specified in DEGREES.
        /// Rotation order: ZXY (Unity's default).
        /// </summary>
        /// <param name="eulerDegrees">Euler angles (X, Y, Z) in degrees</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPQuaternion Euler(FPVector3 eulerDegrees)
        {
            return Euler(eulerDegrees.X, eulerDegrees.Y, eulerDegrees.Z);
        }

        /// <summary>
        /// Creates a quaternion representing a rotation of angleDegrees around axis.
        /// </summary>
        /// <param name="angleDegrees">Rotation angle in DEGREES</param>
        /// <param name="axis">Axis to rotate around (will be normalized)</param>
        public static FPQuaternion AngleAxis(FP angleDegrees, FPVector3 axis)
        {
            axis = axis.Normalized;
            FP angleRad = angleDegrees * FP.Deg2Rad;
            FP halfAngle = angleRad * FP.Half;
            FP sin = FPMath.Sin(halfAngle);
            FP cos = FPMath.Cos(halfAngle);

            return new FPQuaternion(axis.X * sin, axis.Y * sin, axis.Z * sin, cos);
        }

        public static FPQuaternion LookRotation(FPVector3 forward, FPVector3 up)
        {
            forward = forward.Normalized;
            FPVector3 right = FPVector3.Cross(up, forward).Normalized;
            up = FPVector3.Cross(forward, right);

            FP m00 = right.X;
            FP m01 = right.Y;
            FP m02 = right.Z;
            FP m10 = up.X;
            FP m11 = up.Y;
            FP m12 = up.Z;
            FP m20 = forward.X;
            FP m21 = forward.Y;
            FP m22 = forward.Z;

            FP num8 = (m00 + m11) + m22;
            FPQuaternion quaternion;

            if (num8 > FP.Zero)
            {
                FP num = FPMath.Sqrt(num8 + FP.One);
                quaternion.W = num * FP.Half;
                num = FP.Half / num;
                quaternion.X = (m12 - m21) * num;
                quaternion.Y = (m20 - m02) * num;
                quaternion.Z = (m01 - m10) * num;
                return quaternion;
            }

            if ((m00 >= m11) && (m00 >= m22))
            {
                FP num7 = FPMath.Sqrt(((FP.One + m00) - m11) - m22);
                FP num4 = FP.Half / num7;
                quaternion.X = FP.Half * num7;
                quaternion.Y = (m01 + m10) * num4;
                quaternion.Z = (m02 + m20) * num4;
                quaternion.W = (m12 - m21) * num4;
                return quaternion;
            }

            if (m11 > m22)
            {
                FP num6 = FPMath.Sqrt(((FP.One + m11) - m00) - m22);
                FP num3 = FP.Half / num6;
                quaternion.X = (m10 + m01) * num3;
                quaternion.Y = FP.Half * num6;
                quaternion.Z = (m21 + m12) * num3;
                quaternion.W = (m20 - m02) * num3;
                return quaternion;
            }

            FP num5 = FPMath.Sqrt(((FP.One + m22) - m00) - m11);
            FP num2 = FP.Half / num5;
            quaternion.X = (m20 + m02) * num2;
            quaternion.Y = (m21 + m12) * num2;
            quaternion.Z = FP.Half * num5;
            quaternion.W = (m01 - m10) * num2;
            return quaternion;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPQuaternion Slerp(FPQuaternion a, FPQuaternion b, FP t)
        {
            t = FPMath.Clamp(t, FP.Zero, FP.One);

            FP dot = Dot(a, b);

            // If the dot product is negative, slerp won't take the shorter path
            if (dot < FP.Zero)
            {
                b = new FPQuaternion(-b.X, -b.Y, -b.Z, -b.W);
                dot = -dot;
            }

            // If quaternions are very close, use linear interpolation
            if (dot > FP.FromFloat(0.9995f))
            {
                return Lerp(a, b, t);
            }

            FP theta = FPMath.Acos(dot);
            FP sinTheta = FPMath.Sin(theta);

            FP wa = FPMath.Sin((FP.One - t) * theta) / sinTheta;
            FP wb = FPMath.Sin(t * theta) / sinTheta;

            return new FPQuaternion(
                a.X * wa + b.X * wb,
                a.Y * wa + b.Y * wb,
                a.Z * wa + b.Z * wb,
                a.W * wa + b.W * wb
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPQuaternion Lerp(FPQuaternion a, FPQuaternion b, FP t)
        {
            t = FPMath.Clamp(t, FP.Zero, FP.One);
            return new FPQuaternion(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t,
                a.W + (b.W - a.W) * t
            ).Normalized;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPQuaternion Inverse(FPQuaternion q)
        {
            FP lengthSq = q.X * q.X + q.Y * q.Y + q.Z * q.Z + q.W * q.W;
            if (lengthSq > FP.Epsilon)
            {
                FP invLength = FP.One / lengthSq;
                return new FPQuaternion(
                    -q.X * invLength,
                    -q.Y * invLength,
                    -q.Z * invLength,
                    q.W * invLength
                );
            }
            return Identity;
        }

        /// <summary>
        /// Decomposes a quaternion into swing and twist components around the given axis.
        /// Swing is the rotation perpendicular to the axis (e.g., X/Y rotation when axis is Z).
        /// Twist is the rotation around the axis (e.g., Z rotation when axis is Z).
        /// Original rotation = swing * twist (twist applied first, then swing).
        /// </summary>
        /// <param name="rotation">The quaternion to decompose</param>
        /// <param name="twistAxis">The axis to decompose around (should be normalized)</param>
        /// <param name="swing">Output: rotation perpendicular to the axis</param>
        /// <param name="twist">Output: rotation around the axis</param>
        public static void DecomposeSwingTwist(
            FPQuaternion rotation,
            FPVector3 twistAxis,
            out FPQuaternion swing,
            out FPQuaternion twist
        )
        {
            // Project the quaternion's vector part onto the twist axis
            FPVector3 rotationAxis = new FPVector3(rotation.X, rotation.Y, rotation.Z);
            FP dot = FPVector3.Dot(rotationAxis, twistAxis);

            // Twist quaternion: rotation around the twist axis only
            FPVector3 projectedAxis = twistAxis * dot;
            twist = new FPQuaternion(projectedAxis.X, projectedAxis.Y, projectedAxis.Z, rotation.W);

            // Handle near-zero twist (rotation is pure swing)
            FP twistMagSq =
                twist.X * twist.X + twist.Y * twist.Y + twist.Z * twist.Z + twist.W * twist.W;
            if (twistMagSq < FP.Epsilon)
            {
                twist = Identity;
                swing = rotation;
                return;
            }

            // Normalize twist
            twist = twist.Normalized;

            // Swing = rotation * inverse(twist)
            // This gives us the rotation that, when combined with twist, produces the original
            swing = rotation * Inverse(twist);
        }

        /// <summary>
        /// Extracts the swing component (rotation perpendicular to axis) from a quaternion.
        /// For axis = Forward (Z), this extracts the X/Y rotation without the Z rotation.
        /// Useful for separating visual tilt from physics rotation.
        /// </summary>
        /// <param name="rotation">The quaternion to extract swing from</param>
        /// <param name="twistAxis">The axis to remove rotation around (should be normalized)</param>
        /// <returns>The swing component (rotation perpendicular to the axis)</returns>
        public static FPQuaternion ExtractSwing(FPQuaternion rotation, FPVector3 twistAxis)
        {
            DecomposeSwingTwist(rotation, twistAxis, out FPQuaternion swing, out _);
            return swing;
        }

        /// <summary>
        /// Extracts the twist component (rotation around axis) from a quaternion.
        /// For axis = Forward (Z), this extracts just the Z rotation.
        /// </summary>
        /// <param name="rotation">The quaternion to extract twist from</param>
        /// <param name="twistAxis">The axis to extract rotation around (should be normalized)</param>
        /// <returns>The twist component (rotation around the axis)</returns>
        public static FPQuaternion ExtractTwist(FPQuaternion rotation, FPVector3 twistAxis)
        {
            DecomposeSwingTwist(rotation, twistAxis, out _, out FPQuaternion twist);
            return twist;
        }

        // Operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPQuaternion operator *(FPQuaternion a, FPQuaternion b)
        {
            return new FPQuaternion(
                a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
                a.W * b.Y + a.Y * b.W + a.Z * b.X - a.X * b.Z,
                a.W * b.Z + a.Z * b.W + a.X * b.Y - a.Y * b.X,
                a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPVector3 operator *(FPQuaternion rotation, FPVector3 point)
        {
            FP x2 = rotation.X + rotation.X;
            FP y2 = rotation.Y + rotation.Y;
            FP z2 = rotation.Z + rotation.Z;

            FP xx2 = rotation.X * x2;
            FP yy2 = rotation.Y * y2;
            FP zz2 = rotation.Z * z2;
            FP xy2 = rotation.X * y2;
            FP xz2 = rotation.X * z2;
            FP yz2 = rotation.Y * z2;
            FP wx2 = rotation.W * x2;
            FP wy2 = rotation.W * y2;
            FP wz2 = rotation.W * z2;

            return new FPVector3(
                (FP.One - (yy2 + zz2)) * point.X + (xy2 - wz2) * point.Y + (xz2 + wy2) * point.Z,
                (xy2 + wz2) * point.X + (FP.One - (xx2 + zz2)) * point.Y + (yz2 - wx2) * point.Z,
                (xz2 - wy2) * point.X + (yz2 + wx2) * point.Y + (FP.One - (xx2 + yy2)) * point.Z
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FPQuaternion a, FPQuaternion b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FPQuaternion a, FPQuaternion b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W;
        }

        // Interface implementations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(FPQuaternion other)
        {
            return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        }

        public override bool Equals(object obj)
        {
            return obj is FPQuaternion quaternion && Equals(quaternion);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                X.GetHashCode(),
                Y.GetHashCode(),
                Z.GetHashCode(),
                W.GetHashCode()
            );
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z}, {W})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPQuaternion FromFloats(float x, float y, float z, float w)
        {
            return new FPQuaternion(
                FP.FromFloat(x),
                FP.FromFloat(y),
                FP.FromFloat(z),
                FP.FromFloat(w)
            );
        }
    }
}
