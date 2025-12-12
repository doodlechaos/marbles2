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

        public override void Step()
        {
            //TODO: Each step, rotate the GCObj this component is attached to by SpinDegreesPerSecond degrees in the x, y, z axes respectively.
            //TODO: Be careful to make sure the rotation correctly synchronizes with the physics simulation if it has a physics body for this GCObj.
        }
    }
}
