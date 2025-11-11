using SpacetimeDB;
using System;

public static partial class Module
{
    /// <summary>
    /// Clock update reducer - called on schedule to advance game time
    /// </summary>
    [Reducer]
    public static void ClockUpdate(ReducerContext ctx, ClockSchedule schedule)
    {
        try
        {
            OnClockUpdate(ctx);
        }
        catch (Exception ex)
        {
            Log.Error($"Clock update failed: {ex.Message}");
        }
    }

    private static void OnClockUpdate(ReducerContext ctx)
    {
        var baseCfg = GetBaseCfg(ctx);
        var clock = GetClock(ctx);

        // Calculate delta time since last update

        TimeDuration deltaTime = ctx.Timestamp.TimeDurationSince(clock.PrevClockUpdate);
        
        // Clamp the max delta time to 1 second (crucial to avoid a long catchup if stopping and starting locally)
        var deltaSeconds = deltaTime.Microseconds / 1000000.0;
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
        SetClock(ctx, clock);
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

    // Clock helper methods
    private static Clock GetClock(ReducerContext ctx)
    {
        var clockOpt = ctx.Db.Clock.Id.Find(0);
        if (clockOpt.HasValue)
        {
            return clockOpt.Value;
        }

        // Initialize with default values
        var newClock = new Clock
        {
            Id = 0,
            PrevClockUpdate = ctx.Timestamp,
            TickTimeAccumulatorSec = 0.0f
        };
        ctx.Db.Clock.Insert(newClock);
        return newClock;
    }

    private static void SetClock(ReducerContext ctx, Clock clock)
    {
        ctx.Db.Clock.Id.Update(clock);
    }

    // Seq helper methods
    private static ushort GetSeq(ReducerContext ctx)
    {
        var seqOpt = ctx.Db.Seq.Id.Find(0);
        if (seqOpt.HasValue)
        {
            return seqOpt.Value.Value;
        }

        // Initialize with default value
        var newSeq = new Seq { Id = 0, Value = 0 };
        ctx.Db.Seq.Insert(newSeq);
        return 0;
    }

    private static void SetSeq(ReducerContext ctx, ushort value)
    {
        var seq = ctx.Db.Seq.Id.Find(0);
        if (seq.HasValue)
        {
            var updated = seq.Value;
            updated.Value = value;
            ctx.Db.Seq.Id.Update(updated);
        }
        else
        {
            ctx.Db.Seq.Insert(new Seq { Id = 0, Value = value });
        }
    }

    private static void StepSeq(ReducerContext ctx)
    {
        var current = GetSeq(ctx);
        SetSeq(ctx, unchecked((ushort)(current + 1)));
    }

    // Seq extension methods
    private static ushort WrappingSub(ushort a, ushort b)
    {
        return unchecked((ushort)(a - b));
    }

    private static ushort WrappingAdd(ushort a, ushort b)
    {
        return unchecked((ushort)(a + b));
    }

    // Check if a sequence number is behind another (with wrapping)
    private static bool IsBehind(ushort seq, ushort other)
    {
        // Use wrapping subtraction to handle overflow
        ushort diff = unchecked((ushort)(other - seq));
        // If difference is less than half the range, seq is behind
        return diff > 0 && diff < 32768;
    }

    // StepsSinceLastBatch helper methods
    private static ushort GetStepsSinceLastBatch(ReducerContext ctx)
    {
        var opt = ctx.Db.StepsSinceLastBatch.Id.Find(0);
        if (opt.HasValue)
        {
            return opt.Value.Value;
        }

        var newVal = new StepsSinceLastBatch { Id = 0, Value = 0 };
        ctx.Db.StepsSinceLastBatch.Insert(newVal);
        return 0;
    }

    private static void SetStepsSinceLastBatch(ReducerContext ctx, ushort value)
    {
        var opt = ctx.Db.StepsSinceLastBatch.Id.Find(0);
        if (opt.HasValue)
        {
            var updated = opt.Value;
            updated.Value = value;
            ctx.Db.StepsSinceLastBatch.Id.Update(updated);
        }
        else
        {
            ctx.Db.StepsSinceLastBatch.Insert(new StepsSinceLastBatch { Id = 0, Value = value });
        }
    }

    // StepsSinceLastAuthFrame helper methods
    private static ushort GetStepsSinceLastAuthFrame(ReducerContext ctx)
    {
        var opt = ctx.Db.StepsSinceLastAuthFrame.Id.Find(0);
        if (opt.HasValue)
        {
            return opt.Value.Value;
        }

        var newVal = new StepsSinceLastAuthFrame { Id = 0, Value = 0 };
        ctx.Db.StepsSinceLastAuthFrame.Insert(newVal);
        return 0;
    }

    private static void SetStepsSinceLastAuthFrame(ReducerContext ctx, ushort value)
    {
        var opt = ctx.Db.StepsSinceLastAuthFrame.Id.Find(0);
        if (opt.HasValue)
        {
            var updated = opt.Value;
            updated.Value = value;
            ctx.Db.StepsSinceLastAuthFrame.Id.Update(updated);
        }
        else
        {
            ctx.Db.StepsSinceLastAuthFrame.Insert(new StepsSinceLastAuthFrame { Id = 0, Value = value });
        }
    }

    // LastAuthFrameTimestamp helper methods
    private static Timestamp? GetLastAuthFrameTimestamp(ReducerContext ctx)
    {
        var opt = ctx.Db.LastAuthFrameTimestamp.Id.Find(0);
        if (opt.HasValue)
        {
            return opt.Value.LastAuthFrameTime;
        }
        return null;
    }

    private static void SetLastAuthFrameTimestamp(ReducerContext ctx, Timestamp? timestamp)
    {
        var opt = ctx.Db.LastAuthFrameTimestamp.Id.Find(0);
        if (opt.HasValue)
        {
            var updated = opt.Value;
            updated.LastAuthFrameTime = timestamp ?? default;
            ctx.Db.LastAuthFrameTimestamp.Id.Update(updated);
        }
        else
        {
            ctx.Db.LastAuthFrameTimestamp.Insert(new LastAuthFrameTimestamp 
            { 
                Id = 0, 
                LastAuthFrameTime = timestamp ?? default 
            });
        }
    }

    // BaseCfg helper methods
    private static BaseCfg GetBaseCfg(ReducerContext ctx)
    {
        var opt = ctx.Db.BaseCfg.Id.Find(0);
        if (opt.HasValue)
        {
            return opt.Value;
        }

        // Return default configuration
        var defaultCfg = new BaseCfg
        {
            Id = 0,
            ClockIntervalSec = 1.0 / 60.0,
            targetStepsPerSecond = 60,
            physicsStepsPerBatch = 60,
            stepsPerAuthFrame = 3,
            authFrameTimeErrorThresholdSec = 2.0 / 60.0,
            logInputFrameTimes = false,
            logAuthFrameTimeDiffs = false,
            gcCacheAccountTimeoutMinutes = 2.0
        };
        ctx.Db.BaseCfg.Insert(defaultCfg);
        return defaultCfg;
    }

    private static void SetBaseCfg(ReducerContext ctx, BaseCfg baseCfg)
    {
        ctx.Db.BaseCfg.Id.Delete(0);
        ctx.Db.BaseCfg.Insert(baseCfg);
    }

    // Utility methods
    private static uint SaturatingAdd(uint a, uint b)
    {
        if (uint.MaxValue - a < b)
            return uint.MaxValue;
        return a + b;
    }

    private static ushort SaturatingAdd(ushort a, ushort b)
    {
        if (ushort.MaxValue - a < b)
            return ushort.MaxValue;
        return (ushort)(a + b);
    }
}
