using UnityEngine;

/// <summary>
/// Authoring component for SimpleBattleRoyale game mode levels.
/// Place this on the root of a level prefab that should use SimpleBattleRoyale rules.
///
/// Required child objects:
/// - An object with SpawnPipeAuth component (where players spawn)
///
/// The level will be exported and can be loaded into GameCore for server/client simulation.
/// </summary>
public class SimpleBattleRoyaleAuth : GameTileAuthBase
{
    [Tooltip(
        "Optional reference to the spawn pipe for editor validation. The actual pipe is found at runtime by SpawnPipeComponent."
    )]
    public SpawnPipeAuth spawnPipe;


}

