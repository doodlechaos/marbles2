/* using SpacetimeDB;

//TODO: Can't do this yet because we don't have access to the typed query builder for views in csharp
public static partial class Module
{
    [Table(Public = false)]
    public partial struct ActiveAccountCache
    {
        [PrimaryKey]
        public ulong AccountId;
        public Timestamp LastActiveTime;

        public static void Touch(ReducerContext ctx, ulong accountId)
        {
            if (
                ctx.Db.ActiveAccountCache.AccountId.Find(accountId)
                is ActiveAccountCache activeAccountCache
            )
            {
                activeAccountCache.LastActiveTime = ctx.Timestamp;
                ctx.Db.ActiveAccountCache.AccountId.Update(activeAccountCache);
            }
            else
            {
                ctx.Db.ActiveAccountCache.Insert(
                    new ActiveAccountCache { AccountId = accountId, LastActiveTime = ctx.Timestamp }
                );
            }
        }

        [Reducer]
        public static void EvictStale(ReducerContext ctx, AccountCacheEvictionSchedule schedule)
        {
            float maxSecStale = ServerCfgS.Inst(ctx).activeAccountEvectionScanIntervalSec;

            foreach (var entry in ctx.Db.ActiveAccountCache.Iter().ToArray())
            {
                if (entry.LastActiveTime.TimeDurationSince(ctx.Timestamp).ToSeconds() > maxSecStale)
                    ctx.Db.ActiveAccountCache.AccountId.Delete(entry.AccountId);
            }
        }
    }
}
 */