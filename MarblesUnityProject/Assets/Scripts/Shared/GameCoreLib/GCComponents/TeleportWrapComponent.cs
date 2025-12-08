using System;
using FPMathLib;
using MemoryPack;

namespace GameCoreLib
{
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class TeleportWrapComponent : GCComponent
    {
        [MemoryPackOrder(2)]
        public FPVector2 Offset = FPVector2.Zero;
    }
}
