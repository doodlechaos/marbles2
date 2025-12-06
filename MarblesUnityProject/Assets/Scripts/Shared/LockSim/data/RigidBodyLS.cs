namespace LockSim
{
    using System;
    using FPMathLib;
    using MemoryPack;

    public enum BodyType : byte
    {
        Static = 0,
        Dynamic = 1,
    }

    /// <summary>
    /// A rigid body that represents the dynamics (position, velocity, mass) of a physical object.
    /// Shape and material properties are now defined by attached ColliderLS instances.
    /// </summary>
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
                Inertia = FP.One, // Should be set after attaching colliders
                InverseInertia = FP.One,
            };
        }

        /// <summary>
        /// Sets the inertia from a collider's shape and this body's mass.
        /// Call this after attaching a collider to compute proper rotational inertia.
        /// </summary>
        public void SetInertiaFromCollider(in ColliderLS collider)
        {
            if (BodyType == BodyType.Dynamic && Mass > FP.Zero)
            {
                Inertia = collider.ComputeInertia(Mass);
                InverseInertia = Inertia > FP.Zero ? FP.One / Inertia : FP.Zero;
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
                AngularVelocity =
                    AngularVelocity + FPVector2.Cross(contactVector, impulse) * InverseInertia;
            }
        }

        public void ClearForces()
        {
            Force = FPVector2.Zero;
            Torque = FP.Zero;
        }
    }
}
