using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct BidTimeS
    {
        [PrimaryKey]
        private byte Id;

        public long MicrosecondsRemaining;

        public static BidTimeS Inst(ReducerContext ctx)
        {
            var opt = ctx.Db.BidTimeS.Id.Find(0);
            if (opt.HasValue)
            {
                return opt.Value;
            }
            else
            {
                //Todo make the time duration a config value
                return ctx.Db.BidTimeS.Insert(
                    new BidTimeS { Id = 0, MicrosecondsRemaining = 10d.SecondsToMicroseconds() }
                );
            }
        }

        public static void Set(ReducerContext ctx, BidTimeS bidTime)
        {
            ctx.Db.BidTimeS.Id.Delete(0);
            ctx.Db.BidTimeS.Insert(
                new BidTimeS { Id = 0, MicrosecondsRemaining = bidTime.MicrosecondsRemaining }
            );
        }
    }
}
