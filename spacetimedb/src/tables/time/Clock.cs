using SpacetimeDB;

public static partial class Module
{
    [Table(Public = false)]
    public partial struct Clock
    {
        [PrimaryKey]
        public byte Id; //u8
        public Timestamp PrevClockUpdate;
        public float TickTimeAccumulatorSec; //f32

        public static void SetSingleton(ReducerContext ctx, Timestamp prevClockUpdate, float tickTimeAccumulatorSec)
        {
            var opt = ctx.Db.Clock.Id.Find(0);
            if (opt.HasValue)
            {
                Clock updated = opt.Value;
                updated.PrevClockUpdate = prevClockUpdate;
                updated.TickTimeAccumulatorSec = tickTimeAccumulatorSec;
                ctx.Db.Clock.Id.Update(updated);
            }
            else
            {
                ctx.Db.Clock.Insert(new Clock { Id = 0, PrevClockUpdate = prevClockUpdate, TickTimeAccumulatorSec = tickTimeAccumulatorSec });
            }
        }

    }
}

