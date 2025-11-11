
namespace LockSim
{
    using FPMathLib;
    using System;
    using MemoryPack;

    public enum BodyType : byte
    {
        Static = 0,
        Dynamic = 1
    }

    [Serializable]
    [MemoryPackable]
    public partial struct RigidBodyLS
    {
        // Identification
        public int Id;

        // Body type
        public BodyType BodyType;

        // Transform
        public FPVector2 Position;
        public FP Rotation; // Angle in radians

        // Velocity
        public FPVector2 LinearVelocity;
        public FP AngularVelocity;

        // Forces (cleared each frame)
        public FPVector2 Force;
        public FP Torque;

        // Mass properties
        public FP Mass;
        public FP InverseMass;
        public FP Inertia;
        public FP InverseInertia;

        // Material properties
        public FP Restitution; // Bounciness (0 = no bounce, 1 = perfect bounce)
        public FP Friction;

        // Shape data
        public ShapeType ShapeType;
        public BoxShape BoxShape;
        public CircleShape CircleShape;

        public static RigidBodyLS CreateStatic(int id, FPVector2 position, FP rotation)
        {
            return new RigidBodyLS
            {
                Id = id,
                BodyType = BodyType.Static,
                Position = position,
                Rotation = rotation,
                LinearVelocity = FPVector2.Zero,
                AngularVelocity = FP.Zero,
                Force = FPVector2.Zero,
                Torque = FP.Zero,
                Mass = FP.Zero,
                InverseMass = FP.Zero,
                Inertia = FP.Zero,
                InverseInertia = FP.Zero,
                Restitution = FP.FromFloat(0.2f),
                Friction = FP.FromFloat(0.5f)
            };
        }

        public static RigidBodyLS CreateDynamic(int id, FPVector2 position, FP rotation, FP mass)
        {
            FP invMass = mass > FP.Zero ? FP.One / mass : FP.Zero;
            
            return new RigidBodyLS
            {
                Id = id,
                BodyType = BodyType.Dynamic,
                Position = position,
                Rotation = rotation,
                LinearVelocity = FPVector2.Zero,
                AngularVelocity = FP.Zero,
                Force = FPVector2.Zero,
                Torque = FP.Zero,
                Mass = mass,
                InverseMass = invMass,
                Inertia = FP.One,
                InverseInertia = FP.One,
                Restitution = FP.FromFloat(0.2f),
                Friction = FP.FromFloat(0.5f)
            };
        }

        public void SetBoxShape(FP width, FP height)
        {
            ShapeType = ShapeType.Box;
            BoxShape = BoxShape.FromSize(width, height);

            // Calculate inertia for box: I = mass * (width^2 + height^2) / 12
            if (BodyType == BodyType.Dynamic && Mass > FP.Zero)
            {
                FP w2 = width * width;
                FP h2 = height * height;
                Inertia = Mass * (w2 + h2) / FP.FromInt(12);
                InverseInertia = FP.One / Inertia;
            }
        }

        public void SetCircleShape(FP radius)
        {
            ShapeType = ShapeType.Circle;
            CircleShape = new CircleShape(radius);

            // Calculate inertia for circle: I = mass * radius^2 / 2
            if (BodyType == BodyType.Dynamic && Mass > FP.Zero)
            {
                Inertia = Mass * radius * radius * FP.Half;
                InverseInertia = FP.One / Inertia;
            }
        }

        public AABB ComputeAABB()
        {
            if (ShapeType == ShapeType.Box)
            {
                // Compute oriented box corners and find min/max
                FP cos = FPMath.Cos(Rotation);
                FP sin = FPMath.Sin(Rotation);

                FP hw = BoxShape.HalfWidth;
                FP hh = BoxShape.HalfHeight;

                // Compute the maximum extent in each axis
                FP extentX = FPMath.Abs(hw * cos) + FPMath.Abs(hh * sin);
                FP extentY = FPMath.Abs(hw * sin) + FPMath.Abs(hh * cos);

                return new AABB(
                    new FPVector2(Position.X - extentX, Position.Y - extentY),
                    new FPVector2(Position.X + extentX, Position.Y + extentY)
                );
            }
            else // Circle
            {
                FP radius = CircleShape.Radius;
                return new AABB(
                    new FPVector2(Position.X - radius, Position.Y - radius),
                    new FPVector2(Position.X + radius, Position.Y + radius)
                );
            }
        }

        public void ApplyForce(FPVector2 force)
        {
            if (BodyType == BodyType.Dynamic)
            {
                Force = Force + force;
            }
        }

        public void ApplyImpulse(FPVector2 impulse, FPVector2 contactVector)
        {
            if (BodyType == BodyType.Dynamic)
            {
                LinearVelocity = LinearVelocity + impulse * InverseMass;
                AngularVelocity = AngularVelocity + FPVector2.Cross(contactVector, impulse) * InverseInertia;
            }
        }

        public void ClearForces()
        {
            Force = FPVector2.Zero;
            Torque = FP.Zero;
        }
    }
}

