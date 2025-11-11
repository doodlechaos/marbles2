using FPMathLib;
using System.Collections.Generic;

namespace LockSim
{
    /// <summary>
    /// Contains non-serializable runtime data for physics simulation.
    /// This includes temporary data like contacts that are computed each frame.
    /// </summary>
    public class WorldSimulationContext
    {
        // Simulation settings
        public int VelocityIterations = 8;
        public int PositionIterations = 3;

        // Contacts from last step (computed each frame)
        private List<ContactManifold> contacts = new List<ContactManifold>();

        public IReadOnlyList<ContactManifold> Contacts => contacts;

        internal List<ContactManifold> GetContactsMutable() => contacts;

        public void Clear()
        {
            contacts.Clear();
        }
    }
}

