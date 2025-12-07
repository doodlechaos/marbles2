using System;
using FPMathLib;
using MemoryPack;

namespace GameCoreLib
{
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class CircleCollider2DComponent : GCComponent
    {
        [MemoryPackOrder(1)]
        public FP Radius = FP.Half;

        [MemoryPackOrder(2)]
        public FPVector2 Offset = FPVector2.Zero;

        [MemoryPackOrder(3)]
        public bool IsTrigger = false;

        /// <summary>
        /// Physics material friction. Authored from Unity PhysicsMaterial2D.friction.
        /// </summary>
        [MemoryPackOrder(4)]
        public FP Friction = FP.FromFloat(0.5f);

        /// <summary>
        /// Physics material restitution (bounciness). Authored from Unity PhysicsMaterial2D.bounciness.
        /// </summary>
        [MemoryPackOrder(5)]
        public FP Restitution = FP.Zero;
    }
}
