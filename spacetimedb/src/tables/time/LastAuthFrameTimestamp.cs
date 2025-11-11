using SpacetimeDB;

public static partial class Module
{
        [Table(Public = false)]
        public partial struct LastAuthFrameTimestamp
        {
            [PrimaryKey]
            public byte Id; //u8
            public Timestamp LastAuthFrameTime;

        }
}

