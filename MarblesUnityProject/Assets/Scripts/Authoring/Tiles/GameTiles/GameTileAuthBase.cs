using GameCoreLib;
using UnityEngine;

/// <summary>
/// Base class for game tile authoring components.
/// Game tiles have competitive gameplay with bidding and elimination mechanics.
/// </summary>
public abstract class GameTileAuthBase : TileAuthBase
{
    public Rarity AppearanceFrequency;
    public GameBidCfg GameBidCfg = new GameBidCfg();
}
