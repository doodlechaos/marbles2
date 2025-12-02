using GameCoreLib;
using MemoryPack;
using SpacetimeDB;

public static partial class Module
{
    [Reducer]
    public static void A_InsertBid(ReducerContext ctx, ulong accountId, uint bid)
    {
        if (ctx.Db.AccountBid.AccountId.Find(accountId) is AccountBid existingBid)
        {
            existingBid.TotalBid += bid;
            existingBid.LatestBid = bid;

            ctx.Db.AccountBid.AccountId.Update(existingBid);
        }
        else
        {
            AccountBid accountBid = new AccountBid
            {
                AccountId = accountId,
                LatestBid = bid,
                TotalBid = bid,
            };

            ctx.Db.AccountBid.Insert(accountBid);
        }
    }

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
        try
        {
            GameTileBase gameTile = GetRandomGameTile(ctx);
            Log.Info($"Spinning and loading new game tile: {gameTile}");

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
