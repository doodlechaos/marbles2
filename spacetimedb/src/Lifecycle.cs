using SpacetimeDB;

public static partial class Module
{
    [Reducer(ReducerKind.Init)]
    public static void Init(ReducerContext ctx)
    {
        Log.Info($"[Init] Server Starting!");

        ctx.Db.Admin.Insert(new Admin { AdminIdentity = ctx.Identity });

        InitSchedules(ctx);
    }

    private static void InitSchedules(ReducerContext ctx)
    {
        BaseCfg cfg = BaseCfg.GetSingleton(ctx);

        ctx.Db.ClockSchedule.Insert(
            new ClockSchedule
            {
                Id = 0,
                ScheduledAt = new ScheduleAt.Interval(
                    TimeDuration.FromSeconds(cfg.ClockIntervalSec)
                ),
            }
        );
    }

    [Reducer(ReducerKind.ClientConnected)]
    public static void Connect(ReducerContext ctx)
    {
        Log.Info(
            $"[Connect] Client Connecting from sender identity: {ctx.Sender}. Jwt: {ctx.SenderAuth.Jwt}"
        );
        try
        {
            Account account = Account.GetOrCreate(ctx);
            Log.Info(
                $"[Connect] Got/created account ID: {account.Id} for identity: {account.Identity}"
            );
            account.IsConnected = true;
            ctx.Db.Account.Identity.Update(account);
            Log.Info($"[Connect] Successfully updated account IsConnected=true");
        }
        catch (Exception ex)
        {
            Log.Error($"[Connect] Error in Connect reducer: {ex.Message}");
            Log.Error($"[Connect] Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    [Reducer(ReducerKind.ClientDisconnected)]
    public static void Disconnect(ReducerContext ctx)
    {
        Log.Info($"[Init] Client Disconnecting");
        Account account = Account.GetOrCreate(ctx);
        account.IsConnected = false;
        ctx.Db.Account.Identity.Update(account);
    }
}
