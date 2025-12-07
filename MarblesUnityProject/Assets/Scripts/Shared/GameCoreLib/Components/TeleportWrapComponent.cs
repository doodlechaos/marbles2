using System;
using FPMathLib;
using MemoryPack;

namespace GameCoreLib
{
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class TeleportWrapComponent : RuntimeObjComponent
    {
        [MemoryPackOrder(1)]
        public FPVector2 Offset = FPVector2.Zero;
    }
}
