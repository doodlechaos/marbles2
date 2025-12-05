using System;
using System.Runtime.CompilerServices;
using MemoryPack;
using Newtonsoft.Json;

namespace FPMathLib
{
    /// <summary>
    /// Deterministic fixed-point 3D transform similar to Unity's Transform component
    /// Supports position, rotation (quaternion), and scale with parent-child hierarchy
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class FPTransform3D
    {
        // Local space properties
        private FPVector3 _localPosition;
        private FPQuaternion _localRotation;
        private FPVector3 _localScale;

        // Cached world space values
        private FPVector3 _worldPosition;
        private FPQuaternion _worldRotation;
        private FPVector3 _worldScale;
        private bool _worldDirty;

        // Hierarchy
        private FPTransform3D _parent;

        [MemoryPackConstructor]
        public FPTransform3D()
        {
            _localPosition = FPVector3.Zero;
            _localRotation = FPQuaternion.Identity;
            _localScale = FPVector3.One;
            _worldPosition = FPVector3.Zero;
            _worldRotation = FPQuaternion.Identity;
            _worldScale = FPVector3.One;
            _worldDirty = false;
            _parent = null;
        }

        public FPTransform3D(FPVector3 position, FPQuaternion rotation, FPVector3 scale)
        {
            _localPosition = position;
            _localRotation = rotation;
            _localScale = scale;
            _worldPosition = position;
            _worldRotation = rotation;
            _worldScale = scale;
            _worldDirty = false;
            _parent = null;
        }

        #region Local Space Properties
        [JsonProperty("localPosition")]
        [MemoryPackOrder(0)]
        public FPVector3 LocalPosition
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _localPosition;
            set
            {
                _localPosition = value;
                MarkWorldDirty();
            }
        }

        [JsonProperty("localRotation")]
        [MemoryPackOrder(1)]
        public FPQuaternion LocalRotation
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _localRotation;
            set
            {
                _localRotation = value;
                MarkWorldDirty();
            }
        }

        [JsonProperty("localScale")]
        [MemoryPackOrder(2)]
        public FPVector3 LocalScale
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _localScale;
            set
            {
                _localScale = value;
                MarkWorldDirty();
            }
        }

        /// <summary>
        /// Local Euler angles in degrees.
        /// </summary>
        [MemoryPackIgnore]
        public FPVector3 LocalEulerAngles
        {
            get => _localRotation.EulerAngles;
            set => _localRotation = FPQuaternion.Euler(value);
        }

        #endregion

        #region World Space Properties
        [MemoryPackIgnore]
        public FPVector3 Position
        {
            get
            {
                UpdateWorldTransform();
                return _worldPosition;
            }
            set
            {
                if (_parent == null)
                {
                    _localPosition = value;
                    _worldPosition = value;
                }
                else
                {
                    _parent.UpdateWorldTransform();
                    // Convert world position to local space
                    FPVector3 diff = value - _parent._worldPosition;
                    _localPosition = FPQuaternion.Inverse(_parent._worldRotation) * diff;
                    _localPosition = FPVector3.Scale(
                        _localPosition,
                        InverseScale(_parent._worldScale)
                    );
                }
                MarkWorldDirty();
            }
        }

        [MemoryPackIgnore]
        public FPQuaternion Rotation
        {
            get
            {
                UpdateWorldTransform();
                return _worldRotation;
            }
            set
            {
                if (_parent == null)
                {
                    _localRotation = value;
                    _worldRotation = value;
                }
                else
                {
                    _parent.UpdateWorldTransform();
                    _localRotation = FPQuaternion.Inverse(_parent._worldRotation) * value;
                }
                MarkWorldDirty();
            }
        }

        [MemoryPackIgnore]
        public FPVector3 LossyScale
        {
            get
            {
                UpdateWorldTransform();
                return _worldScale;
            }
        }

        /// <summary>
        /// World Euler angles in degrees.
        /// </summary>
        [MemoryPackIgnore]
        public FPVector3 EulerAngles
        {
            get => Rotation.EulerAngles;
            set => Rotation = FPQuaternion.Euler(value);
        }

        #endregion

        #region Direction Vectors
        [MemoryPackIgnore]
        public FPVector3 Forward
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Rotation * FPVector3.Forward;
        }

        [MemoryPackIgnore]
        public FPVector3 Back
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Rotation * FPVector3.Back;
        }

        [MemoryPackIgnore]
        public FPVector3 Up
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Rotation * FPVector3.Up;
        }

        [MemoryPackIgnore]
        public FPVector3 Down
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Rotation * FPVector3.Down;
        }

        [MemoryPackIgnore]
        public FPVector3 Right
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Rotation * FPVector3.Right;
        }

        [MemoryPackIgnore]
        public FPVector3 Left
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Rotation * FPVector3.Left;
        }

        #endregion

        #region Hierarchy

        [MemoryPackIgnore]
        public FPTransform3D Parent
        {
            get => _parent;
            set
            {
                if (_parent == value)
                    return;

                // Store world transform before changing parent
                FPVector3 worldPos = Position;
                FPQuaternion worldRot = Rotation;

                _parent = value;

                // Restore world transform after changing parent
                Position = worldPos;
                Rotation = worldRot;
            }
        }

        #endregion

        #region Transformation Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Translate(FPVector3 translation, bool worldSpace = true)
        {
            if (worldSpace)
            {
                Position = Position + translation;
            }
            else
            {
                LocalPosition = LocalPosition + translation;
            }
        }

        /// <summary>
        /// Applies a rotation to the transform.
        /// </summary>
        /// <param name="eulerAnglesDegrees">Euler angles in DEGREES (X, Y, Z)</param>
        /// <param name="worldSpace">If true, rotates in world space; otherwise in local space</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rotate(FPVector3 eulerAnglesDegrees, bool worldSpace = true)
        {
            FPQuaternion rotation = FPQuaternion.Euler(eulerAnglesDegrees);
            if (worldSpace)
            {
                Rotation = rotation * Rotation;
            }
            else
            {
                LocalRotation = LocalRotation * rotation;
            }
        }

        /// <summary>
        /// Applies a quaternion rotation to the transform.
        /// </summary>
        /// <param name="rotation">The quaternion rotation to apply</param>
        /// <param name="worldSpace">If true, rotates in world space; otherwise in local space</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rotate(FPQuaternion rotation, bool worldSpace = true)
        {
            if (worldSpace)
            {
                Rotation = rotation * Rotation;
            }
            else
            {
                LocalRotation = LocalRotation * rotation;
            }
        }

        /// <summary>
        /// Rotates the transform around a point in world space.
        /// </summary>
        /// <param name="point">The point to rotate around</param>
        /// <param name="axis">The axis to rotate around</param>
        /// <param name="angleDegrees">The rotation angle in DEGREES</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RotateAround(FPVector3 point, FPVector3 axis, FP angleDegrees)
        {
            FPVector3 worldPos = Position;
            FPQuaternion rotation = FPQuaternion.AngleAxis(angleDegrees, axis);
            FPVector3 diff = worldPos - point;
            diff = rotation * diff;
            Position = point + diff;
            Rotation = rotation * Rotation;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LookAt(FPVector3 worldPosition, FPVector3 worldUp)
        {
            FPVector3 forward = (worldPosition - Position).Normalized;
            if (forward.SqrMagnitude > FP.Epsilon)
            {
                Rotation = FPQuaternion.LookRotation(forward, worldUp);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LookAt(FPTransform3D target, FPVector3 worldUp)
        {
            LookAt(target.Position, worldUp);
        }

        #endregion

        #region Space Conversion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FPVector3 TransformPoint(FPVector3 localPoint)
        {
            // Scale -> Rotate -> Translate
            FPVector3 scaled = FPVector3.Scale(localPoint, LocalScale);
            FPVector3 rotated = LocalRotation * scaled;
            return LocalPosition + rotated;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FPVector3 InverseTransformPoint(FPVector3 worldPoint)
        {
            // Inverse: Translate -> Rotate -> Scale
            FPVector3 translated = worldPoint - Position;
            FPVector3 rotated = FPQuaternion.Inverse(Rotation) * translated;
            return FPVector3.Scale(rotated, InverseScale(LossyScale));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FPVector3 TransformDirection(FPVector3 localDirection)
        {
            return LocalRotation * localDirection;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FPVector3 InverseTransformDirection(FPVector3 worldDirection)
        {
            return FPQuaternion.Inverse(Rotation) * worldDirection;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FPVector3 TransformVector(FPVector3 localVector)
        {
            FPVector3 scaled = FPVector3.Scale(localVector, LocalScale);
            return LocalRotation * scaled;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FPVector3 InverseTransformVector(FPVector3 worldVector)
        {
            FPVector3 rotated = FPQuaternion.Inverse(Rotation) * worldVector;
            return FPVector3.Scale(rotated, InverseScale(LossyScale));
        }

        #endregion

        #region Matrix Operations

        /// <summary>
        /// Get the transformation matrix (TRS - Translate, Rotate, Scale)
        /// Returns a 4x4 matrix as a 16-element array in column-major order
        /// </summary>
        public FP[] GetMatrix()
        {
            UpdateWorldTransform();

            FP[] matrix = new FP[16];

            // Extract rotation components
            FP x = _worldRotation.X;
            FP y = _worldRotation.Y;
            FP z = _worldRotation.Z;
            FP w = _worldRotation.W;

            FP x2 = x + x;
            FP y2 = y + y;
            FP z2 = z + z;

            FP xx = x * x2;
            FP xy = x * y2;
            FP xz = x * z2;
            FP yy = y * y2;
            FP yz = y * z2;
            FP zz = z * z2;
            FP wx = w * x2;
            FP wy = w * y2;
            FP wz = w * z2;

            // Column 0
            matrix[0] = (FP.One - (yy + zz)) * _worldScale.X;
            matrix[1] = (xy + wz) * _worldScale.X;
            matrix[2] = (xz - wy) * _worldScale.X;
            matrix[3] = FP.Zero;

            // Column 1
            matrix[4] = (xy - wz) * _worldScale.Y;
            matrix[5] = (FP.One - (xx + zz)) * _worldScale.Y;
            matrix[6] = (yz + wx) * _worldScale.Y;
            matrix[7] = FP.Zero;

            // Column 2
            matrix[8] = (xz + wy) * _worldScale.Z;
            matrix[9] = (yz - wx) * _worldScale.Z;
            matrix[10] = (FP.One - (xx + yy)) * _worldScale.Z;
            matrix[11] = FP.Zero;

            // Column 3 (position)
            matrix[12] = _worldPosition.X;
            matrix[13] = _worldPosition.Y;
            matrix[14] = _worldPosition.Z;
            matrix[15] = FP.One;

            return matrix;
        }

        #endregion

        #region Helper Methods

        private void UpdateWorldTransform()
        {
            if (!_worldDirty)
                return;

            if (_parent == null)
            {
                _worldPosition = _localPosition;
                _worldRotation = _localRotation;
                _worldScale = _localScale;
            }
            else
            {
                _parent.UpdateWorldTransform();

                // World rotation
                _worldRotation = _parent._worldRotation * _localRotation;

                // World scale
                _worldScale = FPVector3.Scale(_parent._worldScale, _localScale);

                // World position
                FPVector3 scaledPos = FPVector3.Scale(_localPosition, _parent._worldScale);
                _worldPosition = _parent._worldPosition + (_parent._worldRotation * scaledPos);
            }

            _worldDirty = false;
        }

        private void MarkWorldDirty()
        {
            _worldDirty = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static FPVector3 InverseScale(FPVector3 scale)
        {
            return new FPVector3(
                scale.X != FP.Zero ? FP.One / scale.X : FP.Zero,
                scale.Y != FP.Zero ? FP.One / scale.Y : FP.Zero,
                scale.Z != FP.Zero ? FP.One / scale.Z : FP.Zero
            );
        }

        #endregion

        public override string ToString()
        {
            return $"Position: {Position}, Rotation: {Rotation}, Scale: {LossyScale}";
        }
    }
}
