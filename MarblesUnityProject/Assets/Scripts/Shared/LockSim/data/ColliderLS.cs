namespace LockSim
{
    using System;
    using FPMathLib;
    using MemoryPack;

    /// <summary>
    /// A collider that defines the shape and material properties for collision detection.
    /// Can be attached to a RigidBodyLS or exist independently (for sensors/static geometry).
    /// </summary>
    [Serializable]
    [MemoryPackable]
    public partial struct ColliderLS
    {
        public const int InvalidBodyId = -1;

        // Identification
        public int Id;

        /// <summary>
        /// The parent body this collider is attached to, or InvalidBodyId for orphan colliders.
        /// </summary>
        public int ParentBodyId;

        // Local transform relative to parent body (or world if no parent)
        public FPVector2 LocalPosition;
        public FP LocalRotation;

        // Shape data
        public ShapeType ShapeType;
        public BoxShape BoxShape;
        public CircleShape CircleShape;

        // Material properties
        public FP Friction;
        public FP Restitution;

        public static ColliderLS CreateBox(
            int id,
            FP width,
            FP height,
            int parentBodyId = InvalidBodyId
        )
        {
            return new ColliderLS
            {
                Id = id,
                ParentBodyId = parentBodyId,
                LocalPosition = FPVector2.Zero,
                LocalRotation = FP.Zero,
                ShapeType = ShapeType.Box,
                BoxShape = BoxShape.FromSize(width, height),
                CircleShape = default,
                Friction = FP.FromFloat(0.5f),
                Restitution = FP.FromFloat(0.2f),
            };
        }

        public static ColliderLS CreateCircle(int id, FP radius, int parentBodyId = InvalidBodyId)
        {
            return new ColliderLS
            {
                Id = id,
                ParentBodyId = parentBodyId,
                LocalPosition = FPVector2.Zero,
                LocalRotation = FP.Zero,
                ShapeType = ShapeType.Circle,
                BoxShape = default,
                CircleShape = new CircleShape(radius),
                Friction = FP.FromFloat(0.5f),
                Restitution = FP.FromFloat(0.2f),
            };
        }

        /// <summary>
        /// Computes the world-space position of this collider given a parent body.
        /// </summary>
        public FPVector2 GetWorldPosition(in RigidBodyLS parentBody)
        {
            if (LocalPosition == FPVector2.Zero)
                return parentBody.Position;

            FP cos = FPMath.Cos(parentBody.Rotation);
            FP sin = FPMath.Sin(parentBody.Rotation);
            FPVector2 rotated = new FPVector2(
                LocalPosition.X * cos - LocalPosition.Y * sin,
                LocalPosition.X * sin + LocalPosition.Y * cos
            );
            return parentBody.Position + rotated;
        }

        /// <summary>
        /// Computes the world-space rotation of this collider given a parent body.
        /// </summary>
        public FP GetWorldRotation(in RigidBodyLS parentBody)
        {
            return parentBody.Rotation + LocalRotation;
        }

        /// <summary>
        /// Computes the AABB in world space given the parent body's transform.
        /// If no parent (orphan collider), use identity transform.
        /// </summary>
        public AABB ComputeAABB(FPVector2 worldPos, FP worldRot)
        {
            if (ShapeType == ShapeType.Box)
            {
                FP cos = FPMath.Cos(worldRot);
                FP sin = FPMath.Sin(worldRot);

                FP hw = BoxShape.HalfWidth;
                FP hh = BoxShape.HalfHeight;

                FP extentX = FPMath.Abs(hw * cos) + FPMath.Abs(hh * sin);
                FP extentY = FPMath.Abs(hw * sin) + FPMath.Abs(hh * cos);

                return new AABB(
                    new FPVector2(worldPos.X - extentX, worldPos.Y - extentY),
                    new FPVector2(worldPos.X + extentX, worldPos.Y + extentY)
                );
            }
            else // Circle
            {
                FP radius = CircleShape.Radius;
                return new AABB(
                    new FPVector2(worldPos.X - radius, worldPos.Y - radius),
                    new FPVector2(worldPos.X + radius, worldPos.Y + radius)
                );
            }
        }

        /// <summary>
        /// Computes the AABB using the attached parent body's transform.
        /// </summary>
        public AABB ComputeAABB(in RigidBodyLS parentBody)
        {
            return ComputeAABB(GetWorldPosition(parentBody), GetWorldRotation(parentBody));
        }

        /// <summary>
        /// Computes inertia contribution for this shape given a mass.
        /// </summary>
        public FP ComputeInertia(FP mass)
        {
            if (mass <= FP.Zero)
                return FP.Zero;

            if (ShapeType == ShapeType.Box)
            {
                FP w = BoxShape.HalfWidth * FP.Two;
                FP h = BoxShape.HalfHeight * FP.Two;
                return mass * (w * w + h * h) / FP.FromInt(12);
            }
            else
            {
                FP r = CircleShape.Radius;
                return mass * r * r * FP.Half;
            }
        }
    }
}
