using System.Collections.Generic;
using FPMathLib;

namespace LockSim
{
    /// <summary>
    /// Contains non-serializable runtime data for physics simulation.
    /// This includes temporary data like contacts and events that are computed each frame.
    /// </summary>
    public class WorldSimulationContext
    {
        // Simulation settings
        public int VelocityIterations = 8;
        public int PositionIterations = 3;

        // Contacts from last step (computed each frame)
        private List<ContactManifold> contacts = new List<ContactManifold>();

        // Collision/Trigger events from last step
        private List<CollisionEvent> collisionEvents = new List<CollisionEvent>();
        private List<CollisionEvent> triggerEvents = new List<CollisionEvent>();

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

        internal List<ContactManifold> GetContactsMutable() => contacts;

        internal List<CollisionEvent> GetCollisionEventsMutable() => collisionEvents;

        internal List<CollisionEvent> GetTriggerEventsMutable() => triggerEvents;

        public void Clear()
        {
            contacts.Clear();
            collisionEvents.Clear();
            triggerEvents.Clear();
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
