using GameCoreLib;
using MemoryPack;
using SpacetimeDB;

public static partial class Module
{
    /// <summary>
    /// Get a random GameTile template from the database.
    /// Returns a default SimpleBattleRoyale if no templates are stored.
    /// </summary>
    public static GameTileBase GetRandomGameTile(ReducerContext ctx)
    {
        // If we have no game tile data stored, create a default one
        if (ctx.Db.GameTileData.Iter().Count() == 0)
        {
            Log.Warn("No GameTileData found in database, creating default SimpleBattleRoyale");
            return CreateDefaultGameTile();
        }

        // Get a random game tile data
        int randomIndex = ctx.Rng.Next(0, ctx.Db.GameTileData.Iter().Count());
        GameTileData gameTileData = ctx.Db.GameTileData.Iter().ToArray()[randomIndex];

        // Deserialize the game tile
        GameTileBase? gameTile = MemoryPackSerializer.Deserialize<GameTileBase>(
            gameTileData.GameTileBinary
        );

        if (gameTile == null)
        {
            Log.Error($"Failed to deserialize GameTile from {gameTileData.UnityPrefabGUID}");
            return CreateDefaultGameTile();
        }

        Log.Info($"Got random gameTile: {gameTileData.TileName}");
        return gameTile;
    }

    /// <summary>
    /// Create a default GameTile for when no templates are available.
    /// </summary>
    private static GameTileBase CreateDefaultGameTile()
    {
        var gameTile = new SimpleBattleRoyale();
        // Create a minimal TileRoot
        gameTile.TileRoot = new GameCoreObj
        {
            Name = "DefaultLevel",
            Children = new List<GameCoreObj>(),
            GameComponents = new List<GCComponent>
            {
                //TODO This isn't assigning a component id correctly here?
                new TileRootComponent { GameModeType = "SimpleBattleRoyale" },
            },
            Transform = new FPMathLib.FPTransform3D(
                FPMathLib.FPVector3.Zero,
                FPMathLib.FPQuaternion.Identity,
                FPMathLib.FPVector3.One
            ),
        };
        return gameTile;
    }
}
