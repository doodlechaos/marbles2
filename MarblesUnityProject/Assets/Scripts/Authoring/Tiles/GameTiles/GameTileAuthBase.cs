using UnityEngine;

/// <summary>
/// Base class for game tile authoring components.
/// Game tiles have competitive gameplay with bidding and elimination mechanics.
/// </summary>
public abstract class GameTileAuthBase : TileAuthBase
{
    public GameCoreLib.Rarity AppearanceFrequency;
    public int MinAuctionSpots = 0;
    public int MaxAuctionSpots = 10;
    public int MaxRaffleDraws = 10;
}
