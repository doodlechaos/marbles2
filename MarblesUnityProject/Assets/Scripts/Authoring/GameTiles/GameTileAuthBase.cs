using UnityEngine;

public abstract class GameTileAuthBase : MonoBehaviour
{
    public PlayerMarbleAuth PlayerMarblePrefab;
    public GameCoreLib.Rarity AppearanceFrequency;
    public int MinAuctionSpots = 0;
    public int MaxAuctionSpots = 10;
    public int MaxRaffleDraws = 10;
}
