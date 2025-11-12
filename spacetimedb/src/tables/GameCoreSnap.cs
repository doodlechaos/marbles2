using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct GameCoreSnap
    {
        [PrimaryKey]
        public byte Id;

        public ushort Seq;

        public byte[] BinaryData;
    }
}

