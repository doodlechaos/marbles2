

using System;

namespace GameCoreLib
{
    public static class Logger
    {
        // Default to Console.WriteLine if not overridden (for non-Unity environments)
        public static Action<string> Log = Console.WriteLine;

        // Optional: Add more levels if needed, e.g., Error, Warning
        public static Action<string> Error = Console.Error.WriteLine;
    }
}
