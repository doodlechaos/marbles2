using System;
using GameCoreLib;
using UnityEngine;

public class MarbleEffectAuth : GameComponentAuth<MarbleEffectComponent>, IMarbleSignalReceiver
{
    [Tooltip("The effect to apply when a marble signal is received")]
    public MarbleEffect Effect;

    protected override MarbleEffectComponent CreateComponent()
    {
        return new MarbleEffectComponent { Effect = Effect };
    }

    public void OnMarbleSignalReceived(MarbleAuth marble)
    {
        Debug.Log("Unity is just for authoring gamecore components, this does nothing here");
    }
}
