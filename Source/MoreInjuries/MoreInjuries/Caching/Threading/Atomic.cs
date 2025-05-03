using System.Threading;

namespace MoreInjuries.Caching.Threading;

/// <summary>
/// Provides additional atomic operations, complementing <see cref="Interlocked"/>.
/// </summary>
/// <remarks>
/// Operations in this class are guaranteed to be lock-free, and exhibit atomic behavior to the calling thread.
/// </remarks>
public static class Atomic
{
    #region IncrementModulo

    /// <summary>
    /// Atomically increments the value stored in the specified location and wraps it around to zero if it exceeds the specified modulo value.
    /// </summary>
    /// <param name="location">A reference to the integer value to increment.</param>
    /// <param name="modulo">The modulo value. If the incremented value exceeds this modulo, it wraps around to zero.</param>
    /// <returns>The original value stored in <paramref name="location"/> before incrementing.</returns>
    public static int IncrementModulo(ref int location, int modulo)
    {
        int original, newValue;
        do
        {
            original = Volatile.Read(ref location);
            newValue = (original + 1) % modulo;
        }
        while (Interlocked.CompareExchange(ref location, newValue, original) != original);
        return original;
    }

    /// <inheritdoc cref="IncrementModulo(ref int, int)"/>"
    public static long IncrementModulo(ref long location, long modulo)
    {
        long original, newValue;
        do
        {
            original = Volatile.Read(ref location);
            newValue = (original + 1) % modulo;
        }
        while (Interlocked.CompareExchange(ref location, newValue, original) != original);
        return original;
    }

    #endregion

    #region WriteMax

    /// <summary>
    /// Atomically writes the maximum of the value stored in the specified location and the specified value.
    /// </summary>
    /// <remarks>
    /// The input values must satisfy the following precondition:
    /// <c>int.MinValue &lt;= location - value - 1 &lt;= int.MaxValue</c>
    /// </remarks>
    /// <param name="location">A reference to the integer value to write to.</param>
    /// <param name="value">The value to write if it is greater than the value stored in <paramref name="location"/>.</param>
    /// <returns>The original value stored in <paramref name="location"/> before writing.</returns>
    public static int WriteMaxFast(ref int location, int value)
    {
        int original, newValue;
        do
        {
            original = Volatile.Read(ref location);
            newValue = FastMath.Max(original, value);
        }
        while (Interlocked.CompareExchange(ref location, newValue, original) != original);
        return original;
    }

    /// <summary>
    /// Atomically writes the maximum of the value stored in the specified location and the specified value.
    /// </summary>
    /// <param name="location">A reference to the integer value to write to.</param>
    /// <param name="value">The value to write if it is greater than the value stored in <paramref name="location"/>.</param>
    /// <returns>The original value stored in <paramref name="location"/> before writing.</returns>
    public static int WriteMax(ref int location, int value)
    {
        int original, newValue;
        do
        {
            original = Volatile.Read(ref location);
            newValue = Math.Max(original, value);
        }
        while (Interlocked.CompareExchange(ref location, newValue, original) != original);
        return original;
    }

    /// <inheritdoc cref="WriteMax(ref int, int)"/>"
    public static long WriteMax(ref long location, long value)
    {
        long original, newValue;
        do
        {
            original = Volatile.Read(ref location);
            newValue = Math.Max(original, value);
        }
        while (Interlocked.CompareExchange(ref location, newValue, original) != original);
        return original;
    }

    #endregion WriteMax

    #region IncrementClampMax

    /// <summary>
    /// Atomically increments the value stored in the specified location, but only if the incremented value is less than or equal to the specified maximum value.
    /// </summary>
    /// <remarks>
    /// The input values must satisfy the following precondition:
    /// <c>int.MinValue &lt;= location - maxValue + 1 &lt;= int.MaxValue</c>
    /// </remarks>
    /// <param name="location">A reference to the integer value to increment.</param>
    /// <param name="maxValue">The maximum value. If the incremented value would exceed this value, it is clamped to this value.</param>
    /// <returns>The original value stored in <paramref name="location"/> before incrementing.</returns>
    public static int IncrementClampMaxFast(ref int location, int maxValue)
    {
        int original, incremented;
        do
        {
            original = Volatile.Read(ref location);
            // believe it or not, this branchless version alone eliminates two branches and three jump labels
            // in the optimized x64 JIT assembly :0
            incremented = FastMath.Min(original + 1, maxValue);
        }
        while (Interlocked.CompareExchange(ref location, incremented, original) != original);
        return original;
    }

    /// <summary>
    /// Atomically increments the value stored in the specified location, but only if the incremented value is less than or equal to the specified maximum value.
    /// </summary>
    /// <param name="location">A reference to the integer value to increment.</param>
    /// <param name="maxValue">The maximum value. If the incremented value would exceed this value, it is clamped to this value.</param>
    /// <returns>The original value stored in <paramref name="location"/> before incrementing.</returns>
    public static int IncrementClampMax(ref int location, int maxValue)
    {
        int original, incremented;
        do
        {
            original = Volatile.Read(ref location);
            incremented = Math.Min(original + 1, maxValue);
        }
        while (Interlocked.CompareExchange(ref location, incremented, original) != original);
        return original;
    }

    /// <inheritdoc cref="IncrementClampMax(ref int, int)"/>"
    public static long IncrementClampMax(ref long location, long maxValue)
    {
        long original, incremented;
        do
        {
            original = Volatile.Read(ref location);
            incremented = Math.Min(original + 1, maxValue);
        }
        while (Interlocked.CompareExchange(ref location, incremented, original) != original);
        return original;
    }

    #endregion IncrementClampMax

    #region DecrementClampMin

    /// <summary>
    /// Atomically decrements the value stored in the specified location, but only if the decremented value is greater than or equal to the specified minimum value.
    /// </summary>
    /// <remarks>
    /// The input values must satisfy the following precondition:
    /// <c>int.MinValue &lt;= location - minValue - 1 &lt;= int.MaxValue</c>
    /// </remarks>
    /// <param name="location">A reference to the integer value to decrement.</param>
    /// <param name="minValue">The minimum value. If the decremented value would be less than this value, it is clamped to this value.</param>
    /// <returns>The original value stored in <paramref name="location"/> before decrementing.</returns>
    public static int DecrementClampMinFast(ref int location, int minValue)
    {
        int original, decremented;
        do
        {
            original = Volatile.Read(ref location);
            decremented = FastMath.Max(original - 1, minValue);
        }
        while (Interlocked.CompareExchange(ref location, decremented, original) != original);
        return original;
    }

    /// <summary>
    /// Atomically decrements the value stored in the specified location, but only if the decremented value is greater than or equal to the specified minimum value.
    /// </summary>
    /// <param name="location">A reference to the integer value to decrement.</param>
    /// <param name="minValue">The minimum value. If the decremented value would be less than this value, it is clamped to this value.</param>
    /// <returns>The original value stored in <paramref name="location"/> before decrementing.</returns>
    public static int DecrementClampMin(ref int location, int minValue)
    {
        int original, decremented;
        do
        {
            original = Volatile.Read(ref location);
            decremented = Math.Max(original - 1, minValue);
        }
        while (Interlocked.CompareExchange(ref location, decremented, original) != original);
        return original;
    }

    /// <inheritdoc cref="DecrementClampMin(ref int, int)"/>"
    public static long DecrementClampMin(ref long location, long minValue)
    {
        long original, decremented;
        do
        {
            original = Volatile.Read(ref location);
            decremented = Math.Max(original - 1, minValue);
        }
        while (Interlocked.CompareExchange(ref location, decremented, original) != original);
        return original;
    }

    #endregion DecrementClampMin

    #region TestAllFlagsExchange

    /// <summary>
    /// Tests whether the specified flags is set in the specified location (<c>(location &amp; flags) == flags</c>), and if so, replaces the value stored in that location with the specified value.
    /// </summary>
    /// <param name="location">The location to test and exchange.</param>
    /// <param name="value">The value to exchange.</param>
    /// <param name="flags">The flags to test for.</param>
    /// <returns>The original value stored in <paramref name="location"/>.</returns>
    public static int TestAllFlagsExchange(ref int location, int value, int flags)
    {
        bool isFlagSet;
        int original;
        do
        {
            original = Volatile.Read(ref location);
            isFlagSet = (original & flags) == flags;
        }
        while (isFlagSet && Interlocked.CompareExchange(ref location, value, original) != original);
        return original;
    }

    /// <inheritdoc cref="TestAllFlagsExchange(ref int, int, int)"/>"
    public static long TestAllFlagsExchange(ref long location, long value, long flags)
    {
        bool isFlagSet;
        long original;
        do
        {
            original = Volatile.Read(ref location);
            isFlagSet = (original & flags) == flags;
        }
        while (isFlagSet && Interlocked.CompareExchange(ref location, value, original) != original);
        return original;
    }

    /// <inheritdoc cref="TestAllFlagsExchange(ref int, int, int)"/>"
    public static nint TestAllFlagsExchange(ref nint location, nint value, nint flags)
    {
        bool isFlagSet;
        nint original;
        do
        {
            original = Volatile.Read(ref location);
            isFlagSet = (original & flags) == flags;
        }
        while (isFlagSet && Interlocked.CompareExchange(ref location, value, original) != original);
        return original;
    }

    #endregion TestAllFlagsExchange

    #region TryTestAllFlagsExchange

    /// <summary>
    /// Tests whether the specified flags is set in the specified location (<c>(location &amp; flags) == flags</c>), and if so, replaces the value stored in that location with the specified value.
    /// </summary>
    /// <param name="location">The location to test and exchange.</param>
    /// <param name="value">The value to exchange.</param>
    /// <param name="flags">The flags to test for.</param>
    /// <returns><see langword="true"/> if original value stored in <paramref name="location"/> was replaced; otherwise, <see langword="false"/>.</returns>
    public static bool TryTestAllFlagsExchange(ref int location, int value, int flags)
    {
        bool isFlagSet;
        int original;
        do
        {
            original = Volatile.Read(ref location);
            isFlagSet = (original & flags) == flags;
        }
        while (isFlagSet && Interlocked.CompareExchange(ref location, value, original) != original);
        return isFlagSet;
    }

    /// <inheritdoc cref="TryTestAllFlagsExchange(ref int, int, int)"/>"
    public static bool TryTestAllFlagsExchange(ref long location, long value, long flags)
    {
        bool isFlagSet;
        long original;
        do
        {
            original = Volatile.Read(ref location);
            isFlagSet = (original & flags) == flags;
        }
        while (isFlagSet && Interlocked.CompareExchange(ref location, value, original) != original);
        return isFlagSet;
    }

    /// <inheritdoc cref="TryTestAllFlagsExchange(ref int, int, int)"/>"
    public static bool TryTestAllFlagsExchange(ref nint location, nint value, nint flags)
    {
        bool isFlagSet;
        nint original;
        do
        {
            original = Volatile.Read(ref location);
            isFlagSet = (original & flags) == flags;
        }
        while (isFlagSet && Interlocked.CompareExchange(ref location, value, original) != original);
        return isFlagSet;
    }

    #endregion TryTestAllFlagsExchange

    #region TestAnyFlagsExchange

    /// <summary>
    /// Tests whether any of the specified flags are set in the specified location (<c>(location &amp; flags) != 0</c>), and if so, replaces the value stored in that location with the specified value.
    /// </summary>
    /// <param name="location">The location to test and exchange.</param>
    /// <param name="value">The value to exchange.</param>
    /// <param name="flags">The flags to test for.</param>
    /// <returns>The original value stored in <paramref name="location"/>.</returns>
    public static int TestAnyFlagsExchange(ref int location, int value, int flags)
    {
        bool isFlagSet;
        int original;
        do
        {
            original = Volatile.Read(ref location);
            isFlagSet = (original & flags) != 0;
        }
        while (isFlagSet && Interlocked.CompareExchange(ref location, value, original) != original);
        return original;
    }

    /// <inheritdoc cref="TestAnyFlagsExchange(ref int, int, int)"/>"
    public static long TestAnyFlagsExchange(ref long location, long value, long flags)
    {
        bool isFlagSet;
        long original;
        do
        {
            original = Volatile.Read(ref location);
            isFlagSet = (original & flags) != 0;
        }
        while (isFlagSet && Interlocked.CompareExchange(ref location, value, original) != original);
        return original;
    }

    /// <inheritdoc cref="TestAnyFlagsExchange(ref int, int, int)"/>"
    public static nint TestAnyFlagsExchange(ref nint location, nint value, nint flags)
    {
        bool isFlagSet;
        nint original;
        do
        {
            original = Volatile.Read(ref location);
            isFlagSet = (original & flags) != 0;
        }
        while (isFlagSet && Interlocked.CompareExchange(ref location, value, original) != original);
        return original;
    }

    #endregion TestAnyFlagsExchange

    #region TryTestAnyFlagsExchange

    /// <summary>
    /// Tests whether any of the specified flags are set in the specified location (<c>(location &amp; flags) != 0</c>), and if so, replaces the value stored in that location with the specified value.
    /// </summary>
    /// <param name="location">The location to test and exchange.</param>
    /// <param name="value">The value to exchange.</param>
    /// <param name="flags">The flags to test for.</param>
    /// <returns><see langword="true"/> if original value stored in <paramref name="location"/> was replaced; otherwise, <see langword="false"/>.</returns>
    public static bool TryTestAnyFlagsExchange(ref int location, int value, int flags)
    {
        bool isFlagSet;
        int original;
        do
        {
            original = Volatile.Read(ref location);
            isFlagSet = (original & flags) != 0;
        }
        while (isFlagSet && Interlocked.CompareExchange(ref location, value, original) != original);
        return isFlagSet;
    }

    /// <inheritdoc cref="TryTestAnyFlagsExchange(ref int, int, int)"/>"
    public static bool TryTestAnyFlagsExchange(ref long location, long value, long flags)
    {
        bool isFlagSet;
        long original;
        do
        {
            original = Volatile.Read(ref location);
            isFlagSet = (original & flags) != 0;
        }
        while (isFlagSet && Interlocked.CompareExchange(ref location, value, original) != original);
        return isFlagSet;
    }

    #endregion TryTestAnyFlagsExchange
}