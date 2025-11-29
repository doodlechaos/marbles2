using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct AccountSeq
    {
        [PrimaryKey]
        public ulong IdS;

        public ulong Seq;
    }

    [Reducer]
    public static void UpsertAccountSeq(ReducerContext ctx, AccountSeq row)
    {
        ctx.Db.AccountSeq.IdS.Delete(row.IdS);
        ctx.Db.AccountSeq.Insert(row);
    }
}

public static class AccountSeqHelper
{
    public static ulong IncAndGet(ReducerContext ctx)
    {
        var accountSeqOpt = ctx.Db.AccountSeq.IdS.Find(0);
        Module.AccountSeq accountSeq;

        if (accountSeqOpt.HasValue)
        {
            accountSeq = accountSeqOpt.Value;
        }
        else
        {
            accountSeq = ctx.Db.AccountSeq.Insert(new Module.AccountSeq { IdS = 0, Seq = 0 });
        }

        accountSeq.Seq = accountSeq.Seq + 1;
        var newSeq = ctx.Db.AccountSeq.IdS.Update(accountSeq);
        return newSeq.Seq;
    }
}
