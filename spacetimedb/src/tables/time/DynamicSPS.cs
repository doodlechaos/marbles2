using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct DynamicSPS
    {
        [PrimaryKey]
        private byte Id;
        public Timestamp LastBatchStartTime;
        public float ActualTPS;

        public static void SetSingleton(ReducerContext ctx, Timestamp lastBatchStartTime, float actualTPS)
        {
            ctx.Db.DynamicSPS.Id.Delete(0);
            ctx.Db.DynamicSPS.Insert(new DynamicSPS { Id = 0, LastBatchStartTime = lastBatchStartTime, ActualTPS = actualTPS });
        }

        public static DynamicSPS GetSingleton(ReducerContext ctx)
        {
            var opt = ctx.Db.DynamicSPS.Id.Find(0);
            if (opt.HasValue)
            {
                return opt.Value;
            }
            else
            {
                DynamicSPS dynamicSPS = new DynamicSPS { Id = 0, LastBatchStartTime = default, ActualTPS = 0 };
                SetSingleton(ctx, default, 0);
                return dynamicSPS;
            }
        }
    }


}

