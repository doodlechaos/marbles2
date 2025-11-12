using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct DynamicSPS
    {
        [PrimaryKey]
        public byte Id;

        public Timestamp LastBatchStartTime;

        public float ActualTPS;

    }
}

