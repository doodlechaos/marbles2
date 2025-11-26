public static class MathExtensions
{
    /// <summary>
    /// Subtracts the amount from the value, returning 0 if the result would be negative.
    /// </summary>
    public static ulong SaturatingSub(this ulong value, ulong amount)
    {
        // If the amount we want to subtract is greater than what we have, return 0.
        // Otherwise, perform the subtraction.
        return amount > value ? 0 : value - amount;
    }
}
