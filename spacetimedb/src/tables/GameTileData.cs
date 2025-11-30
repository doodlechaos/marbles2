using GameCoreLib;
using MemoryPack;
using SpacetimeDB;

public static partial class Module
{
    /// <summary>
    /// Stores pre-serialized GameTile templates that can be loaded into tile slots.
    /// The GameTile is exported directly from Unity, eliminating the need for JSON parsing.
    /// </summary>
    [Table(Public = true)]
    public partial struct GameTile
    {
        [PrimaryKey]
        public string UnityPrefabGUID;

        /// <summary>
        /// Level name for display purposes
        /// </summary>
        public string LevelName;

        /// <summary>
        /// Rarity for weighted random selection
        /// </summary>
        public int Rarity;

        public int MinAuctionSpots;
        public int MaxAuctionSpots;
        public int MaxRaffleDraws;

        /// <summary>
        /// Serialized GameTileBase (e.g., SimpleBattleRoyale).
        /// Deserialize with MemoryPack and call Initialize(tileId) to use.
        /// </summary>
        public byte[] GameTileBinary;
    }

    [Reducer]
    public static void UpsertGameTile(ReducerContext ctx, GameTile gameTileData)
    {
        // Delete existing entry if it exists, then insert the new one (upsert behavior)
        ctx.Db.GameTile.UnityPrefabGUID.Delete(gameTileData.UnityPrefabGUID);
        ctx.Db.GameTile.Insert(gameTileData);
    }
}
