using GameCoreLib;
using UnityEngine;

public class AutoSpinAuth : GameComponentAuth<AutoSpinComponent>
{
    public Vector3 SpinDegreesPerSecond = new Vector3(0, 0, 8f);

    protected override AutoSpinComponent CreateComponent()
    {
        return new AutoSpinComponent { SpinDegreesPerSecond = SpinDegreesPerSecond.ToFPVector3() };
    }
}
