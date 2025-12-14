using System;
using FPMathLib;
using MemoryPack;

namespace GameCoreLib
{
    /// <summary>
    /// Abstract base class for all 2D collider components.
    /// Contains shared properties for physics collision detection.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    [MemoryPackUnion(0, typeof(BoxCollider2DComponent))]
    [MemoryPackUnion(1, typeof(CircleCollider2DComponent))]
    public abstract partial class Collider2DComponent : GCComponent
    {
        /// <summary>
        /// Physics collider ID in the LockSim world. -1 means not yet registered.
        /// Set by RuntimePhysicsBuilder when the collider is created.
        /// </summary>
        [MemoryPackOrder(2)]
        public int PhysicsColliderId = -1;

        /// <summary>
        /// Offset from the object's position.
        /// </summary>
        [MemoryPackOrder(3)]
        public FPVector2 Offset = FPVector2.Zero;

        /// <summary>
        /// If true, this collider detects overlaps but doesn't resolve physics.
        /// </summary>
        [MemoryPackOrder(4)]
        public bool IsTrigger = false;

        /// <summary>
        /// Physics material friction. Authored from Unity PhysicsMaterial2D.friction.
        /// </summary>
        [MemoryPackOrder(5)]
        public FP Friction = FP.FromFloat(0.5f);

        /// <summary>
        /// Physics material restitution (bounciness). Authored from Unity PhysicsMaterial2D.bounciness.
        /// </summary>
        [MemoryPackOrder(6)]
        public FP Restitution = FP.Zero;

        /// <summary>
        /// True if this collider has been registered with the physics system.
        /// </summary>
        [MemoryPackIgnore]
        public bool HasPhysicsCollider => PhysicsColliderId >= 0;

        /// <summary>
        /// Sets the enabled state of this collider in the physics simulation.
        /// </summary>
        public void SetColliderEnabled(LockSim.World sim, bool enabled)
        {
            if (sim == null || !HasPhysicsCollider)
                return;

            sim.SetColliderEnabled(PhysicsColliderId, enabled);
        }
    }
}
