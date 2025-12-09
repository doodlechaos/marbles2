public static class UIntExtensions
{
    public static uint SaturatingAdd(this uint a, uint b)
    {
        // If b would push a past MaxValue, clamp.
        if (b > uint.MaxValue - a)
            return uint.MaxValue;

        return a + b;
    }

    public static uint SaturatingAdd(this uint a, int b)
    {
        // Do the math in 64-bit so we never overflow.
        long result = (long)a + b;

        if (result < 0L)
            return 0u;

        if (result > uint.MaxValue)
            return uint.MaxValue;

        return (uint)result;
    }
}
