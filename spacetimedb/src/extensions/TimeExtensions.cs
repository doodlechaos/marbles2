/// <summary>
/// Extension methods for working with time values in microseconds since Unix epoch.
/// </summary>
public static class TimeExtensions
{
    // Day calculation constants
    private const long DAY_US = 86_400_000_000L; // 24h in microseconds

    /// <summary>
    /// Gets the day index that increments exactly at 00:00:00 UTC.
    /// </summary>
    public static long GetDayIndex(this long microsUtc)
    {
        // Use proper Euclidean division for negative values
        return microsUtc >= 0 ? microsUtc / DAY_US : (microsUtc - DAY_US + 1) / DAY_US;
    }

    /// <summary>
    /// Gets the start-of-day (00:00 UTC) in microseconds for the day index represented by this value.
    /// </summary>
    public static long ToStartOfDayMicroseconds(this long dayIdx)
    {
        return dayIdx * DAY_US;
    }

    /// <summary>
    /// Gets the next midnight ≥ this time, in microseconds since epoch.
    /// </summary>
    public static long NextMidnight(this long microsNow)
    {
        long today = microsNow.GetDayIndex();
        return (today + 1).ToStartOfDayMicroseconds();
    }

    /// <summary>
    /// Gets the start of the current day (00:00 UTC) in microseconds.
    /// </summary>
    public static long StartOfDay(this long microsUtc)
    {
        return microsUtc.GetDayIndex().ToStartOfDayMicroseconds();
    }

    /// <summary>
    /// Converts microseconds to seconds.
    /// </summary>
    public static double ToSeconds(this long micros)
    {
        return micros / 1_000_000.0;
    }

    /// <summary>
    /// Converts microseconds to minutes.
    /// </summary>
    public static double ToMinutes(this long micros)
    {
        return micros / (60.0 * 1_000_000.0);
    }

    /// <summary>
    /// Converts seconds to microseconds.
    /// </summary>
    public static long SecondsToMicroseconds(this double seconds)
    {
        return (long)(seconds * 1_000_000.0);
    }

    /// <summary>
    /// Converts minutes to microseconds.
    /// </summary>
    public static long MinutesToMicroseconds(this double minutes)
    {
        return (long)(minutes * 60.0 * 1_000_000.0);
    }
}
