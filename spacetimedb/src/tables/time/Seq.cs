using SpacetimeDB;

public static partial class Module
{
        [SpacetimeDB.Table(Public = false)]
        public partial struct Seq
        {
            [PrimaryKey]
            public byte id; //u8
            public ushort seq; //u16

        }
}

