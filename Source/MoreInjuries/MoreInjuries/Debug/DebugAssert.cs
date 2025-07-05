using System.Diagnostics;
using System.Runtime.CompilerServices;

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
    public static void IsTrue(bool condition, [CallerArgumentExpression(nameof(condition))] string? message = null)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message ?? "Assertion failed");
        }
    }
}
