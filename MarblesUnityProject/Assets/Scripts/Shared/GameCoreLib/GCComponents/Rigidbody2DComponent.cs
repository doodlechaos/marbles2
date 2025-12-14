using System;
using FPMathLib;
using LockSim;
using MemoryPack;

namespace GameCoreLib
{
    /// <summary>
    /// Rigidbody component for 2D physics simulation.
    /// Stores the physics body ID and provides physics-related operations.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class Rigidbody2DComponent : GCComponent
    {
        /// <summary>
        /// Physics body ID in the LockSim world. -1 means not yet registered.
        /// Set by RuntimePhysicsBuilder when the physics body is created.
        /// </summary>
        [MemoryPackOrder(2)]
        public int PhysicsBodyId = -1;

        [MemoryPackOrder(3)]
        public Rigidbody2DType BodyType = Rigidbody2DType.Dynamic;

        [MemoryPackOrder(4)]
        public FP Mass = FP.One;

        [MemoryPackOrder(5)]
        public FP LinearDrag = FP.Zero;

        [MemoryPackOrder(6)]
        public FP AngularDrag = FP.FromFloat(0.05f);

        [MemoryPackOrder(7)]
        public FP GravityScale = FP.One;

        [MemoryPackOrder(8)]
        public bool FreezeRotation = false;

        /// <summary>
        /// True if this rigidbody has been registered with the physics system.
        /// </summary>
        [MemoryPackIgnore]
        public bool HasPhysicsBody => PhysicsBodyId >= 0;

        /// <summary>
        /// Sets the world position of this rigidbody, syncing with the physics simulation.
        /// </summary>
        public void SetWorldPosition(World sim, FPVector3 newPosition, bool resetVelocity = true)
        {
            if (sim == null || !HasPhysicsBody)
                return;

            // Update transform position
            if (GCObj != null)
            {
                GCObj.Transform.Position = newPosition;
            }

            try
            {
                var body = sim.GetBody(PhysicsBodyId);
                body.Position = new FPVector2(newPosition.X, newPosition.Y);

                if (resetVelocity)
                {
                    body.LinearVelocity = FPVector2.Zero;
                    body.AngularVelocity = FP.Zero;
                }

                sim.SetBody(PhysicsBodyId, body);
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"Rigidbody2DComponent.SetWorldPosition: Failed to update physics body: {e.Message}"
                );
            }
        }

        /// <summary>
        /// Gets the current physics body from the simulation.
        /// </summary>
        public bool TryGetBody(World sim, out RigidBodyLS body)
        {
            if (sim == null || !HasPhysicsBody)
            {
                body = default;
                return false;
            }
            return sim.TryGetBody(PhysicsBodyId, out body);
        }

        /// <summary>
        /// Updates the physics body in the simulation.
        /// </summary>
        public void SetBody(World sim, RigidBodyLS body)
        {
            if (sim == null || !HasPhysicsBody)
                return;

            sim.SetBody(PhysicsBodyId, body);
        }
    }

    public enum Rigidbody2DType
    {
        Dynamic = 0,
        Kinematic = 1,
        Static = 2,
    }
}
