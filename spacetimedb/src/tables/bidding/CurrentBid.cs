using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct CurrentBid
    {
        public ulong Value;
    }
}
