using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct BidConfigS
    {
        [PrimaryKey]
        private byte Id;

        public int MinAuctionSpots;
        public int MaxAcutionSpots;
        public int MaxRaffleDraws;

        public static BidConfigS Inst(ReducerContext ctx)
        {
            var opt = ctx.Db.BidConfigS.Id.Find(0);
            if (opt.HasValue)
            {
                return opt.Value;
            }
            else
            {
                return ctx.Db.BidConfigS.Insert(
                    new BidConfigS
                    {
                        Id = 0,
                        MinAuctionSpots = 0,
                        MaxAcutionSpots = 0,
                        MaxRaffleDraws = 0,
                    }
                );
            }
        }
    }
}
