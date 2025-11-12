using SpacetimeDB;

public static class TimeDurationExtensions
{
    public static double ToSeconds(this TimeDuration timeDuration)
    {
        return timeDuration.Microseconds / 1000000.0;
    }
}
