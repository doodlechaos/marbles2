using GameCoreLib;
using UnityEngine;

/// <summary>
/// Authors a defense wall that manages child DefenseBrickAuth components.
/// Provides a RebuildWall function to reinitialize all bricks with starting health.
/// </summary>
public class DefenseWallAuth : GameComponentAuth<DefenseWallComponent>
{
    [Tooltip("The starting health for each brick when the wall is rebuilt.")]
    public int brickStartingHealth = 1;

    protected override DefenseWallComponent CreateComponent()
    {
        return new DefenseWallComponent { BrickStartingHealth = brickStartingHealth };
    }
}
