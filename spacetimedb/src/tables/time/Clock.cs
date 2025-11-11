using SpacetimeDB;

public static partial class Module
{
        [SpacetimeDB.Table(Public = false)]
        public partial struct Clock
        {
            [PrimaryKey]
            public byte id; //u8
            public Timestamp prevClockUpdate;
            public float tickTimeAccumulatorSec; //f32

        }
}

