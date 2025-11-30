using GameCoreLib;
using MemoryPack;
using SpacetimeDB;

public static partial class Module
{
    [Reducer]
    public static void CloseAndCycleGameTile(ReducerContext ctx, byte worldId)
    {
        Log.Info($"Closing and cycling game tile for world {worldId}");
        try
        {
            byte[] eventData = new InputEvent.StartCloseDoorAnimation(worldId).ToBinary();
            Log.Info($"Serialized event data: {eventData.Length} bytes");
            //1. Start door close animation
            ctx.Db.InputCollector.Insert(
                new InputCollector { delaySeqs = 0, inputEventData = eventData }
            );

            GameTileBase gameTile = GetRandomGameTile(ctx);

            //2. Schedule a load game tile input event
            byte[] eventData2 = new InputEvent.LoadGameTile(worldId, gameTile).ToBinary();

            Log.Info($"Serialized event data 2: {eventData2.Length} bytes");
            ctx.Db.InputCollector.Insert(
                new InputCollector { delaySeqs = 120, inputEventData = eventData2 }
            );
        }
        catch (Exception e)
        {
            Log.Error($"Error closing and cycling game tile: {e.Message}");
            return;
        }
    }

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

        Log.Info($"Loaded GameTile: {gameTileData.LevelName}");
        return gameTile;
    }

    /// <summary>
    /// Create a default GameTile for when no templates are available.
    /// </summary>
    private static GameTileBase CreateDefaultGameTile()
    {
        var gameTile = new SimpleBattleRoyale();
        // Create a minimal TileRoot
        gameTile.TileRoot = new RuntimeObj
        {
            Name = "DefaultLevel",
            Children = new List<RuntimeObj>(),
            GameComponents = new List<GameComponent>
            {
                new LevelRootComponent { GameModeType = "SimpleBattleRoyale" },
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
