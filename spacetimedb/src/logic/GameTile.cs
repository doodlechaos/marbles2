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

            LevelFile levelFile = GetRandomLevelFile(ctx);

            //2. Schedule a load level file input event
            byte[] eventData2 = new InputEvent.LoadLevelFile(worldId, levelFile).ToBinary();

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

    public static LevelFile GetRandomLevelFile(ReducerContext ctx)
    {
        //If we have no level file datas stored, create a default one
        if (ctx.Db.LevelFileData.Iter().Count() == 0)
        {
            return LevelFile.Default;
        }

        //Get a random level file data
        int randomIndex = ctx.Rng.Next(0, ctx.Db.LevelFileData.Iter().Count());
        LevelFileData levelFileData = ctx.Db.LevelFileData.Iter().ToArray()[randomIndex];

        //Deserialize the level file data into a level file
        LevelFile levelFile =
            MemoryPackSerializer.Deserialize<LevelFile>(levelFileData.LevelFileBinary)
            ?? LevelFile.Default;
        return levelFile;
    }
}
