using System;
using SpacetimeDB;

namespace GameCoreLib
{
    [Serializable]
    [SpacetimeDB.Type]
    public partial struct GameBidCfg
    {
        public ushort MinAuctionSpots;
        public ushort MaxAcutionSpots;
        public ushort MaxRaffleDraws;

        public GameBidCfg(ushort minAuctionSpots, ushort maxAcutionSpots, ushort maxRaffleDraws)
        {
            MinAuctionSpots = minAuctionSpots;
            MaxAcutionSpots = maxAcutionSpots;
            MaxRaffleDraws = maxRaffleDraws;
        }
    }
}
