using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MoreInjuries.Debug;

internal static class DebugAssert
{
    [Conditional("DEBUG")]
    public static void IsNotNull<T>([NotNull] T? obj, [CallerArgumentExpression(nameof(obj))] string? name = null)
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
            throw new InvalidOperationException(message!);
        }
    }

    [Conditional("DEBUG")]
    public static void IsFalse(bool condition, [CallerArgumentExpression(nameof(condition))] string? message = null)
    {
        if (condition)
        {
            throw new InvalidOperationException(message!);
        }
    }
}
