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
        BaseCfgS cfg = BaseCfgS.Inst(ctx);

        ctx.Db.ClockSchedule.Insert(
            new ClockSchedule
            {
                Id = 0,
                ScheduledAt = new ScheduleAt.Interval(
                    TimeDuration.FromSeconds(cfg.ClockIntervalSec)
                ),
            }
        );

        //TODO: I think we can update these so that if I change the cfg interval time value at runtime it can live update the schedule without having to restart the server
        ctx.Db.DeterminismSnapSchedule.Insert(
            new DeterminismSnapSchedule
            {
                Id = 0,
                ScheduledAt = new ScheduleAt.Interval(
                    TimeDuration.FromSeconds(cfg.DeterminismSnapIntervalSec)
                ),
            }
        );
    }

    [Reducer(ReducerKind.ClientConnected)]
    public static void Connect(ReducerContext ctx)
    {
        AuthCtx auth = ctx.SenderAuth;

        if (ctx.Db.Session.Identity.Find(ctx.Sender).HasValue)
        {
            Log.Error(
                $"[Connect] Client already connected. Ignoring connection for identity: {ctx.Sender}"
            );
            return;
        }

        Session session = new Session
        {
            Identity = ctx.Sender,
            HasJwt = auth.HasJwt,
            Issuer = auth.Jwt?.Issuer ?? "",
            Subject = auth.Jwt?.Subject ?? "",
            ConnectedAt = ctx.Timestamp,
        };

        if (auth.IsInternal)
            session.Kind = SessionKind.Internal;
        else if (
            auth.Jwt is JwtClaims claims
            && claims.Issuer == "https://auth.spacetimedb.com/oidc"
        )
            session.Kind = SessionKind.SpacetimeAuth;
        else
            session.Kind = SessionKind.Anonymous;

        ctx.Db.Session.Insert(session);

        Log.Info($"[Connect] Session connecting: {session}");

        if (session.Kind == SessionKind.SpacetimeAuth)
        {
            Account account = Account.GetOrCreate(ctx);
            Log.Info(
                $"[Connect] Got/created account ID: {account.Id} for identity: {account.Identity}"
            );
            account.IsConnected = true;
            ctx.Db.Account.Identity.Update(account);
            Log.Info($"[Connect] Successfully updated account IsConnected=true");
        }
    }

    [Reducer(ReducerKind.ClientDisconnected)]
    public static void Disconnect(ReducerContext ctx)
    {
        Log.Info($"[Disconnect] Disconnecting: {ctx.Sender}");
        if (ctx.Db.Session.Identity.Find(ctx.Sender) is Session session)
        {
            Log.Info($"[Disconnect] Session disconnecting: {session}");
            ctx.Db.Session.Identity.Delete(ctx.Sender);
        }

        if (ctx.Db.Account.Identity.Find(ctx.Sender) is Account account)
        {
            account.IsConnected = false;
            ctx.Db.Account.Identity.Update(account);
            Log.Info($"[Disconnect] Successfully updated account IsConnected=false");
        }
    }
}
