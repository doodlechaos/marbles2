using System;
using SpacetimeDB;

public static partial class Module
{
    private const long MicrosPerDay = 86_400_000_000L; // 24*60*60*1_000_000

    /// <summary>
    /// Clock update reducer - called on schedule to advance game time
    /// </summary>
    [Reducer]
    public static void ClockUpdate(ReducerContext ctx, ClockSchedule schedule)
    {
        var baseCfg = BaseCfgS.Inst(ctx);
        var clock = Clock.GetSingleton(ctx);

        // Calculate delta time since last update
        TimeDuration deltaTime = ctx.Timestamp.TimeDurationSince(clock.PrevClockUpdate);

        // Clamp the max delta time to 1 second (crucial to avoid a long catchup if stopping and starting locally)
        var deltaSeconds = deltaTime.ToSeconds();
        if (deltaSeconds > 1.0)
        {
            Log.Warn("Clock delta time is greater than 1 second, clamping to 1 second");
            deltaSeconds = 1.0;
        }

        clock.TickTimeAccumulatorSec += (float)deltaSeconds;

        // Process steps based on accumulated time
        float secPerStep = 1.0f / baseCfg.targetStepsPerSecond;
        while (clock.TickTimeAccumulatorSec >= secPerStep)
        {
            clock.TickTimeAccumulatorSec -= secPerStep;
            UpdateBidManager(ctx, secPerStep);
            OnSeqTick(ctx);
        }

        clock.PrevClockUpdate = ctx.Timestamp;
        Clock.Set(ctx, clock.PrevClockUpdate, clock.TickTimeAccumulatorSec);
    }
}
