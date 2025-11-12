using SpacetimeDB;

public static partial class Module
{
    [Table(Public = false)]
    public partial struct LastAuthFrameTimestamp
    {
        [PrimaryKey]
        private byte Id; //u8
        public Timestamp LastAuthFrameTime;

        public static void Set(ReducerContext ctx, Timestamp lastAuthFrameTime)
        {
            ctx.Db.LastAuthFrameTimestamp.Id.Delete(0);
            ctx.Db.LastAuthFrameTimestamp.Insert(new LastAuthFrameTimestamp { Id = 0, LastAuthFrameTime = lastAuthFrameTime });
        }

        public static LastAuthFrameTimestamp GetSingleton(ReducerContext ctx)
        {
            var opt = ctx.Db.LastAuthFrameTimestamp.Id.Find(0);
            if (opt.HasValue)
            {
                return opt.Value;
            }
            else
            {
                LastAuthFrameTimestamp lastAuthFrameTimestamp = new LastAuthFrameTimestamp { Id = 0, LastAuthFrameTime = default };
                Set(ctx, default);
                return lastAuthFrameTimestamp;
            }
        }
    }
}

