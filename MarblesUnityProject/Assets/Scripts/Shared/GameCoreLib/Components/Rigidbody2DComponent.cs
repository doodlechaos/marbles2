using System;
using FPMathLib;
using MemoryPack;

namespace GameCoreLib
{
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class Rigidbody2DComponent : RuntimeObjComponent
    {
        [MemoryPackOrder(1)]
        public Rigidbody2DType BodyType = Rigidbody2DType.Dynamic;

        [MemoryPackOrder(2)]
        public FP Mass = FP.One;

        [MemoryPackOrder(3)]
        public FP LinearDrag = FP.Zero;

        [MemoryPackOrder(4)]
        public FP AngularDrag = FP.FromFloat(0.05f);

        [MemoryPackOrder(5)]
        public FP GravityScale = FP.One;

        [MemoryPackOrder(6)]
        public bool FreezeRotation = false;
    }

    public enum Rigidbody2DType
    {
        Dynamic = 0,
        Kinematic = 1,
        Static = 2,
    }
}
