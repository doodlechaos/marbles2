using SpacetimeDB;

public static partial class Module
{
    [Table(Public = false)]
    public partial struct Seq
    {
        [PrimaryKey]
        public byte Id; //u8
        public ushort Value; //u16

    }
}

