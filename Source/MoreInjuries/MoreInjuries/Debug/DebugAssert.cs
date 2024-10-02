using System.Diagnostics;

namespace MoreInjuries.Debug;

internal static class DebugAssert
{
    [Conditional("DEBUG")]
    public static void NotNull(object? obj, string name)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(name);
        }
    }

    [Conditional("DEBUG")]
    public static void IsTrue(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
