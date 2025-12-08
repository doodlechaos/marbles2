using System;
using System.Collections.Generic;
using MemoryPack;

namespace GameCoreLib
{
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class MarbleDetectorComponent : GCComponent { }
}
