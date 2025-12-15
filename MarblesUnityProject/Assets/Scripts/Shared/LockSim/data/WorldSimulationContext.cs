using System.Collections.Generic;
using FPMathLib;

namespace LockSim
{
    /// <summary>
    /// Contains non-serializable runtime data for physics simulation.
    /// This includes temporary data like contacts and events that are computed each frame.
    ///
    /// Snapshot/restore: All data here is scratch/transient. On restore, the broadphase
    /// is rebuilt deterministically from serialized world state.
    /// </summary>
    public class WorldSimulationContext
    {
        // Simulation settings
        public int VelocityIterations = 8;
        public int PositionIterations = 3;

        // Broadphase scratch data (rebuilt each frame from world state)
        // Default cell size of 2 units works well for typical game objects
        private static readonly FP DefaultCellSize = FP.FromInt(2);
        private BroadPhase broadPhase;

        // Contacts from last step (computed each frame)
        private readonly List<ContactManifold> contacts = new List<ContactManifold>();

        // Collision/Trigger events from last step
        private readonly List<CollisionEvent> collisionEvents = new List<CollisionEvent>();
        private readonly List<CollisionEvent> triggerEvents = new List<CollisionEvent>();

        // Reusable scratch arrays for narrow phase (avoids per-pair allocations)
        internal readonly FPVector2[] ScratchCornersA = new FPVector2[4];
        internal readonly FPVector2[] ScratchCornersB = new FPVector2[4];
        internal readonly FPVector2[] ScratchAxes = new FPVector2[4];

        public IReadOnlyList<ContactManifold> Contacts => contacts;

        /// <summary>
        /// All collision events (Enter/Stay/Exit) from the last physics step.
        /// Only includes events for solid (non-trigger) collider pairs.
        /// </summary>
        public IReadOnlyList<CollisionEvent> CollisionEvents => collisionEvents;

        /// <summary>
        /// All trigger events (Enter/Stay/Exit) from the last physics step.
        /// Only includes events where at least one collider is a trigger.
        /// </summary>
        public IReadOnlyList<CollisionEvent> TriggerEvents => triggerEvents;

        /// <summary>
        /// Gets the broad phase, creating it lazily if needed.
        /// </summary>
        internal BroadPhase GetBroadPhase()
        {
            broadPhase ??= new BroadPhase(DefaultCellSize);
            return broadPhase;
        }

        internal List<ContactManifold> GetContactsMutable() => contacts;

        internal List<CollisionEvent> GetCollisionEventsMutable() => collisionEvents;

        internal List<CollisionEvent> GetTriggerEventsMutable() => triggerEvents;

        public void Clear()
        {
            contacts.Clear();
            collisionEvents.Clear();
            triggerEvents.Clear();
            broadPhase?.Clear();
        }

        /// <summary>
        /// Clears only the events, keeping contacts intact.
        /// </summary>
        internal void ClearEvents()
        {
            collisionEvents.Clear();
            triggerEvents.Clear();
        }
    }
}
