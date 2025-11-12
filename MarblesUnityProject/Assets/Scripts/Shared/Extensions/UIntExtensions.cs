public static class UIntExtensions
{
    public static uint SaturatingAdd(this uint a, uint b)
    {
        // If b would push a past MaxValue, clamp.
        if (b > uint.MaxValue - a)
            return uint.MaxValue;

        return a + b;
    }
}