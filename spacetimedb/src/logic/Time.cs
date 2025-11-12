using System;
using SpacetimeDB;

public static partial class Module
{
    /// <summary>
    /// Clock update reducer - called on schedule to advance game time
    /// </summary>
    [Reducer]
    public static void ClockUpdate(ReducerContext ctx, ClockSchedule schedule)
    {
        var baseCfg = BaseCfg.GetSingleton(ctx);
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
            OnSeqTick(ctx);
        }

        clock.PrevClockUpdate = ctx.Timestamp;
        Clock.Set(ctx, clock.PrevClockUpdate, clock.TickTimeAccumulatorSec);
    }


    // Day calculation constants
    private const long DAY_US = 86_400_000_000L; // 24h in microseconds

    /// <summary>
    /// Day index that increments exactly at 00:00:00 UTC.
    /// </summary>
    public static long DayIndexUtcMidnight(long microsUtc)
    {
        // Use Math.DivRem for proper Euclidean division
        return microsUtc >= 0
            ? microsUtc / DAY_US
            : (microsUtc - DAY_US + 1) / DAY_US;
    }

    /// <summary>
    /// Start-of-day (00:00 UTC) for a given day index.
    /// </summary>
    public static long StartOfDayUtcMidnight(long dayIdx)
    {
        return dayIdx * DAY_US;
    }

    /// <summary>
    /// Next midnight ≥ now, in microseconds since epoch.
    /// </summary>
    public static long NextMidnightUtc(long microsNow)
    {
        long today = DayIndexUtcMidnight(microsNow);
        return StartOfDayUtcMidnight(today + 1);
    }

    /// <summary>
    /// Convert minutes to microseconds
    /// </summary>
    public static long MinutesToMicroseconds(double minutes)
    {
        return (long)(minutes * 60.0 * 1_000_000.0);
    }



}
