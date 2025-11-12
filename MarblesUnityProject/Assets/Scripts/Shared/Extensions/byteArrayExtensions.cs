using System.IO;

public static class ByteArrayExtensions
{
    public static string GetDeterministicHashHex(this byte[] data)
    {
        const ulong offset = 14695981039346656037UL;
        const ulong prime = 1099511628211UL;
        ulong h = offset;
        for (int i = 0; i < data.Length; i++)
        {
            h ^= data[i];
            h *= prime;
        }
        return h.ToString("x16"); // 16 hex chars, zero-padded
    }
}
