using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct AccountBid
    {
        [PrimaryKey]
        public ulong AccountId;

        public uint LatestBid;
        public uint TotalBid;
    }
}
