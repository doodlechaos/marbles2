using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct BaseCfg
    {
        [PrimaryKey]
        public byte Id;

        public double ClockIntervalSec; // Seconds between clock ticks
        
        // Physics
        public ushort targetStepsPerSecond;
        public ushort physicsStepsPerBatch;
        public ushort stepsPerAuthFrame;
        public double authFrameTimeErrorThresholdSec;
        public bool logInputFrameTimes;
        public bool logAuthFrameTimeDiffs;
        public double gcCacheAccountTimeoutMinutes;
    }

    [Reducer]
    public static void UpsertBaseCfg(ReducerContext ctx, BaseCfg row)
    {
        ctx.Db.BaseCfg.Id.Delete(row.Id);
        ctx.Db.BaseCfg.Insert(row);
    }
}

