using System;
using FPMathLib;
using MemoryPack;

namespace GameCoreLib
{
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class BoxCollider2DComponent : GCComponent
    {
        [MemoryPackOrder(2)]
        public FPVector2 Size = FPVector2.One;

        [MemoryPackOrder(3)]
        public FPVector2 Offset = FPVector2.Zero;

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
    }
}
