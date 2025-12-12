using System;
using FPMathLib;
using MemoryPack;

namespace GameCoreLib
{
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class AutoSpinComponent : GCComponent
    {
        [MemoryPackOrder(2)]
        public FPVector3 SpinDegreesPerSecond;

        //TODO: Each step, rotate the GCObj this component is attached to by SpinDegreesPerSecond degrees around the Z axis.
        //We need to be careful to also apply the rotation to the 
    }
}
