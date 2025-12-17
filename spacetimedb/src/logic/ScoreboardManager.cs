using SpacetimeDB;

public static partial class Module
{
    /*     public static void GenerateScoreboardEntries(ReducerContext ctx, GameResults gameResults){ TODO: Get the GameResults from the gametile when it finishes (create method for finishing a gametile)
            
        } */

    public static void ApplyScoreboardToAccounts(ReducerContext ctx)
    {
        GTScoreboardS scoreboard = GTScoreboardS.Inst(ctx);
        foreach (ScoreboardEntry entry in scoreboard.Entries)
        {
            if (Account.TryGetById(ctx, entry.AccountId) is Account account)
            {
                account.Points = account.Points.SaturatingAdd(entry.PointsEarned);
                ctx.Db.Account.Identity.Update(account);
            }
        }
    }
}
