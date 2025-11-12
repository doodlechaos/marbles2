using SpacetimeDB;

public static partial class Module
{
    [Table(Public = false)]
    public partial struct Clock
    {
        [PrimaryKey]
        private byte Id; //u8
        public Timestamp PrevClockUpdate;
        public float TickTimeAccumulatorSec; //f32


        public static void Set(ReducerContext ctx, Timestamp prevClockUpdate, float tickTimeAccumulatorSec)
        {
            ctx.Db.Clock.Id.Delete(0);
            ctx.Db.Clock.Insert(new Clock { Id = 0, PrevClockUpdate = prevClockUpdate, TickTimeAccumulatorSec = tickTimeAccumulatorSec });
        }

        public static Clock GetSingleton(ReducerContext ctx)
        {
            var opt = ctx.Db.Clock.Id.Find(0);
            if (opt.HasValue)
            {
                return opt.Value;
            }
            else
            {
                Clock clock = new Clock { Id = 0, PrevClockUpdate = default, TickTimeAccumulatorSec = 0 };
                Set(ctx, default, 0);
                return clock;
            }
        }

    }
}

