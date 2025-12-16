using SpacetimeDB;

public static partial class Module
{
    [Table(Public = true)]
    public partial struct BaseCfgS
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

        public ushort dayRewardStreak1Marbles;
        public ushort dayRewardStreak2Marbles;
        public ushort dayRewardStreak3Marbles;
        public ushort dayRewardStreak4Marbles;
        public ushort dayRewardStreak5Marbles;
        public ushort dayRewardStreak6Marbles;
        public ushort dayRewardStreak7Marbles;

        public static BaseCfgS Inst(ReducerContext ctx)
        {
            if (ctx.Db.BaseCfgS.Id.Find(0) is BaseCfgS opt)
            {
                return opt;
            }
            else
            {
                return ctx.Db.BaseCfgS.Insert(
                    new BaseCfgS
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

                        dayRewardStreak1Marbles = 40,
                        dayRewardStreak2Marbles = 50,
                        dayRewardStreak3Marbles = 60,
                        dayRewardStreak4Marbles = 70,
                        dayRewardStreak5Marbles = 80,
                        dayRewardStreak6Marbles = 90,
                        dayRewardStreak7Marbles = 100,
                    }
                );
            }
        }
    }

    [Reducer]
    public static void UpsertBaseCfg(ReducerContext ctx, BaseCfgS row)
    {
        ctx.Db.BaseCfgS.Id.Delete(row.Id);
        ctx.Db.BaseCfgS.Insert(row);
    }
}
