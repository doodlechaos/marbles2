using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct ScoreboardEntry
    {
        [PrimaryKey]
        public ulong AccountId;
        public int PointsEarned;
    }
}
