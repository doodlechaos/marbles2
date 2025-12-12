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
            Logger.Log($"Stepping AutoSpinComponent for {GCObj.Name}");
            //TODO: Each step, rotate the GCObj this component is attached to by SpinDegreesPerSecond degrees in the x, y, z axes respectively.
            //TODO: Be careful to make sure the rotation correctly synchronizes with the physics simulation if it has a physics body for this GCObj.
            FP stepSeconds = FP.FromFloat(1 / 60f);
            FPVector3 stepDegrees = SpinDegreesPerSecond * stepSeconds;

            // Rotate around local axes so physics and children stay in sync with the transform hierarchy
            Transform?.Rotate(stepDegrees, worldSpace: false);
        }
    }
}
