using FPMathLib;
using GameCoreLib;
using UnityEngine;

/// <summary>
/// Marks a GameObject as a spawn pipe where players enter the game.
/// Attach this to a pipe prefab in Unity - it will be exported to GameCore
/// and can be referenced in game modes like SimpleBattleRoyale.
/// </summary>
public class SpawnPipeAuth : GameComponentAuth<SpawnPipeComponent>
{
    [Tooltip("Delay in seconds between spawning each player")]
    public float spawnDelay = 0.5f;

    protected override SpawnPipeComponent CreateComponent()
    {
        return new SpawnPipeComponent { SpawnDelay = FP.FromFloat(spawnDelay) };
    }
}

