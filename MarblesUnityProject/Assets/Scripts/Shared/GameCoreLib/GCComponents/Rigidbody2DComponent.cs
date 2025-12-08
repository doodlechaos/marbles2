using System;
using FPMathLib;
using MemoryPack;

namespace GameCoreLib
{
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class Rigidbody2DComponent : GCComponent
    {
        [MemoryPackOrder(2)]
        public Rigidbody2DType BodyType = Rigidbody2DType.Dynamic;

        [MemoryPackOrder(3)]
        public FP Mass = FP.One;

        [MemoryPackOrder(4)]
        public FP LinearDrag = FP.Zero;

        [MemoryPackOrder(5)]
        public FP AngularDrag = FP.FromFloat(0.05f);

        [MemoryPackOrder(6)]
        public FP GravityScale = FP.One;

        [MemoryPackOrder(7)]
        public bool FreezeRotation = false;
    }

    public enum Rigidbody2DType
    {
        Dynamic = 0,
        Kinematic = 1,
        Static = 2,
    }
}
