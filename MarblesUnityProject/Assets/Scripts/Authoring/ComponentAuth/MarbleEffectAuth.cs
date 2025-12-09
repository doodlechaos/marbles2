using System;
using GameCoreLib;
using UnityEngine;

public class MarbleEffectAuth : GameComponentAuth<MarbleEffectComponent>, IMarbleSignalReceiver
{
    public MarbleEffect Effect;

    public void ApplyEffect(MarbleAuth marble) { }

    public void OnMarbleSignalReceived(MarbleAuth marble)
    {
        Debug.Log("Unity is just for authoring gamecore components, this does nothing here");
    }
}
