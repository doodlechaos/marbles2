using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct DeterminismCheck
    {
        [PrimaryKey]
        public byte Id;

        public ushort Seq;
        public string HashString;
    }
}

