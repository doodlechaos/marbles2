using GameCoreLib;
using MemoryPack;
using SpacetimeDB;

public static partial class Module
{
    /*     [Reducer]
        public static void A_SetGameplayFinished(ReducerContext ctx, bool isGameplayFinished)
        {
            GameplayFinishedFlagS state = new GameplayFinishedFlagS
            {
                Id = 0,
                IsGameplayFinished = isGameplayFinished,
            };
            ctx.Db.GameplayFinishedFlagS.Id.Update(state);
        } */

    [Reducer]
    public static void A_GiveMarbles(ReducerContext ctx, ulong accountId, uint marbles)
    {
        Account? accountOpt = ctx.Db.Account.Id.Find(accountId);
        if (!accountOpt.HasValue)
        {
            Log.Error($"[Admin.GiveMarbles] Account {accountId} not found");
            return;
        }

        Account account = accountOpt.Value;
        account.Marbles += marbles;

        ctx.Db.Account.Id.Update(account);
    }

    [Reducer]
    public static void A_SpinLoadGameplayTile(ReducerContext ctx, byte worldId)
    {
        Log.Info($"Spinning and loading new game tile");
        try
        {
            GameTileBase gameTile = GetRandomGameTile(ctx);

            //2. Schedule a load game tile input event
            byte[] eventData2 = new InputEvent.SpinToNewGameTile(gameTile, worldId).ToBinary();

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
}
