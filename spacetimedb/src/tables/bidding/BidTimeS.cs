using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct BidTimeS
    {
        public ulong MicrosecondsRemaining;

        public static BidTimeS Inst(ReducerContext ctx)
        {
            BidTimeS? opt = ctx.Db.BidTimeS.Iter().FirstOrDefault();
            if (opt != null)
            {
                return opt.Value;
            }
            else
            {
                return new BidTimeS { MicrosecondsRemaining = 0 };
            }
        }

        public static void Set(ReducerContext ctx, BidTimeS bidTime)
        {
            foreach (var item in ctx.Db.BidTimeS.Iter())
                ctx.Db.BidTimeS.Delete(item);

            ctx.Db.BidTimeS.Insert(bidTime);
        }
    }
}
