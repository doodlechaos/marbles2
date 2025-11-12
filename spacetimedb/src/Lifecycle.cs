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
        var baseCfg = GetBaseCfg(ctx);

        ctx.Db.ClockSchedule.Insert(
            new ClockSchedule
            {
                Id = 0,
                ScheduledAt = new ScheduleAt.Interval(TimeDuration.FromSeconds(baseCfg.ClockIntervalSec))
            }
        );
    }

    [Reducer]
    public static void Connect(ReducerContext ctx)
    {
        Log.Info($"[Init] Client Connecting");

    }

    [Reducer(ReducerKind.ClientDisconnected)]
    public static void Disconnect(ReducerContext ctx)
    {
        Log.Info($"[Init] Client Disconnecting");

    }

}
