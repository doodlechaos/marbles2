using System;
using GameCoreLib;
using UnityEngine;

public class MarbleEffectAuth : GameComponentAuth<MarbleEffectComponent>
{
    [Tooltip("The effect to apply when a marble signal is received")]
    public MarbleEffect Effect;

    protected override MarbleEffectComponent CreateComponent()
    {
        return new MarbleEffectComponent { Effect = Effect };
    }


}
