using FPMathLib;
using MemoryPack;

namespace GameCoreLib
{
    /// <summary>
    /// Holds physics data for a RuntimeObj that has a physics body.
    /// </summary>
    [MemoryPackable]
    public partial struct PhysicsBinding
    {
        public int BodyId;
        public int ColliderId;

        /// <summary>
        /// The swing component (X/Y tilt without Z) of the original rotation.
        /// Used to preserve visual rotation while applying physics Z rotation.
        /// Only needed for dynamic bodies; static bodies can leave it as identity.
        /// </summary>
        public FPQuaternion BaseSwing;
    }
}
