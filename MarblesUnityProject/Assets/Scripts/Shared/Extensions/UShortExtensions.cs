// MarblesUnityProject/Assets/Scripts/Shared/Extensions/UShortExtensions.cs
using System;
using System.Runtime.CompilerServices;

public static class UShortExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ushort Wrap16(int v) => unchecked((ushort)(v & 0xFFFF));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ushort Wrap16(uint v) => (ushort)(v & 0xFFFF);

    /// <summary> (a + b) mod 2^16 </summary>
    public static ushort WrappingAdd(this ushort a, ushort b) =>
        Wrap16((uint)a + (uint)b);

    /// <summary> (a - b) mod 2^16 </summary>
    public static ushort WrappingSub(this ushort a, ushort b) =>
        Wrap16((uint)a - (uint)b);

    /// <summary> (a + delta) mod 2^16 — signed delta overload for convenience </summary>
    public static ushort WrappingAdd(this ushort a, int delta) =>
        Wrap16((int)a + delta);

    /// <summary> (a - delta) mod 2^16 — signed delta overload for convenience </summary>
    public static ushort WrappingSub(this ushort a, int delta) =>
        Wrap16((int)a - delta);

    /// <summary>
    /// Shortest signed difference from self to other on the u16 ring.
    /// Returns value in [-32768, 32767] matching JS implementation.
    /// Equivalent to: ((other - self) mod 65536), then map to signed range.
    /// </summary>
    public static short ClosestDiffTo(this ushort self, ushort other)
    {
        int s = self & 0xFFFF;
        int o = other & 0xFFFF;
        int diff = (o - s) & 0xFFFF;
        if (diff > 0x7FFF)
            diff -= 0x10000;
        return (short)diff;
    }

    public static bool IsAhead(this ushort self, ushort other) =>
        self.ClosestDiffTo(other) < 0;

    public static bool IsBehind(this ushort self, ushort other) =>
        self.ClosestDiffTo(other) > 0;

    public static bool IsBehindOrEqual(this ushort self, ushort other) =>
        self.ClosestDiffTo(other) >= 0;

    public static bool IsAheadOrEqual(this ushort self, ushort other) =>
        self.ClosestDiffTo(other) <= 0;

    /// <summary>
    /// Linear interpolate on the u16 ring toward 'other' by factor t.
    /// Matches JS: (self + closestDiff*selfToOther * t) truncated, then masked.
    /// </summary>
    public static ushort LerpTo(this ushort self, ushort other, float t)
    {
        short diff = self.ClosestDiffTo(other);
        int val = (int)(self + diff * t); // truncates toward zero like JS bitwise ops
        return Wrap16(val);
    }

    // Optional double overload
    public static ushort LerpTo(this ushort self, ushort other, double t)
    {
        short diff = self.ClosestDiffTo(other);
        int val = (int)(self + diff * t);
        return Wrap16(val);
    }
}
