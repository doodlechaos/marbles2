using GameCoreLib;
using MemoryPack;
using SpacetimeDB;

public static partial class Module
{
    [Reducer]
    public static void A_SetGameplayFinished(ReducerContext ctx, bool isGameplayFinished)
    {
        GameplayFinishedFlagS state = new GameplayFinishedFlagS
        {
            Id = 0,
            IsGameplayFinished = isGameplayFinished,
        };
        ctx.Db.GameplayFinishedFlagS.Id.Update(state);
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
}
