using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct BaseCfg
    {
        [PrimaryKey]
        public byte Id;

        public double ClockIntervalSec; // Seconds between clock ticks
        public double DeterminismSnapIntervalSec; // Seconds between determinism snaps

        // Physics
        public ushort targetStepsPerSecond;
        public ushort physicsStepsPerBatch;
        public ushort stepsPerAuthFrame;
        public double authFrameTimeErrorThresholdSec;
        public bool logInputFrameTimes;
        public bool logAuthFrameTimeDiffs;
        public double gcCacheAccountTimeoutMinutes;

        public static void SetSingleton(ReducerContext ctx, BaseCfg baseCfg)
        {
            baseCfg.Id = 0;
            ctx.Db.BaseCfg.Id.Delete(0);
            ctx.Db.BaseCfg.Insert(baseCfg);
        }

        public static BaseCfg GetSingleton(ReducerContext ctx)
        {
            var opt = ctx.Db.BaseCfg.Id.Find(0);
            if (opt.HasValue)
            {
                return opt.Value;
            }
            else
            {
                BaseCfg baseCfg = new BaseCfg
                {
                    Id = 0,
                    ClockIntervalSec = 1f / 60f,
                    DeterminismSnapIntervalSec = 10,
                    targetStepsPerSecond = 60,
                    physicsStepsPerBatch = 60,
                    stepsPerAuthFrame = 3,
                    authFrameTimeErrorThresholdSec = 2.0 / 60.0,
                    logInputFrameTimes = false,
                    logAuthFrameTimeDiffs = false,
                    gcCacheAccountTimeoutMinutes = 2.0,
                };
                SetSingleton(ctx, baseCfg);
                return baseCfg;
            }
        }
    }

    [Reducer]
    public static void UpsertBaseCfg(ReducerContext ctx, BaseCfg row)
    {
        ctx.Db.BaseCfg.Id.Delete(row.Id);
        ctx.Db.BaseCfg.Insert(row);
    }
}
