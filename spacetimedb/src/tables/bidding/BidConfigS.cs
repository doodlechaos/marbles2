using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct BidConfigS
    {
        public int MinAuctionSpots;
        public int MaxAcutionSpots;
        public int MaxRaffleDraws;

        public static BidConfigS Inst(ReducerContext ctx)
        {
            BidConfigS? opt = ctx.Db.BidConfigS.Iter().FirstOrDefault();
            if (opt != null)
            {
                return opt.Value;
            }
            else
            {
                return new BidConfigS
                {
                    MinAuctionSpots = 0,
                    MaxAcutionSpots = 0,
                    MaxRaffleDraws = 0,
                };
            }
        }
    }
}
