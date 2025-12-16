using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct BiddingStateS
    {
        [PrimaryKey]
        public byte Id;

        /// <summary>
        /// True when the OTHER tile (not CurrBidWorldId) has entered Bidding state,
        /// meaning users will have something to bid on once we start gameplay on the current tile.
        /// This ensures seamless bidding continuity.
        /// </summary>
        public bool OtherTileReadyForBidding;

        /// <summary>
        /// Which tile is currently accepting bids (1 or 2).
        /// The other tile should be in Gameplay, ScoreScreen, or transitioning.
        /// </summary>
        public byte CurrBidWorldId;

        public int MinAuctionSpots;
        public int MaxAcutionSpots;
        public int MaxRaffleDraws;

        public static BiddingStateS Inst(ReducerContext ctx)
        {
            var opt = ctx.Db.BiddingStateS.Id.Find(0);
            if (opt.HasValue)
            {
                return opt.Value;
            }
            else
            {
                return ctx.Db.BiddingStateS.Insert(
                    new BiddingStateS
                    {
                        Id = 0,
                        OtherTileReadyForBidding = false,
                        CurrBidWorldId = 1,
                    }
                );
            }
        }
    }
}
