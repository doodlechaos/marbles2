using UnityEngine;

/// <summary>
/// Authoring component for the throne room tile.
/// The throne tile displays the current king and provides a visual throne room environment.
/// </summary>
public class ThroneTileAuthBase : TileAuthBase
{
    public ThroneAuth Throne;

    //public List<DefenseBrick> DefenseBricks; Don't do this until we have working throne capture mechanics
    public SpawnPipeAuth SpawnPipe;
}
