using System.Runtime.CompilerServices;

namespace MoreInjuries.Caching.Threading;

/// <summary>
/// Provides performance-oriented implementations of common math functions that are generally faster 
/// than the BCL implementations in the <see cref="Math"/> API, but some additional preconditions may apply.
/// </summary>
public static class FastMath
{
    /// <summary>
    /// Calculates the minimum of <paramref name="x"/> and <paramref name="y"/> where <c>int.MinValue &lt;= x - y &lt;= int.MaxValue</c>
    /// </summary>
    /// <param name="x">x, where <c>int.MinValue &lt;= x - y &lt;= int.MaxValue</c></param>
    /// <param name="y">y, where <c>int.MinValue &lt;= x - y &lt;= int.MaxValue</c></param>
    /// <returns>The minimum of <paramref name="x"/> and <paramref name="y"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Min(int x, int y) =>
        y + ((x - y) & ((x - y) >> 31));

    /// <summary>
    /// Calculates the minimum of <paramref name="x"/> and <paramref name="y"/> where <c>long.MinValue &lt;= x - y &lt;= long.MaxValue</c>
    /// </summary>
    /// <param name="x">x, where <c>long.MinValue &lt;= x - y &lt;= long.MaxValue</c></param>
    /// <param name="y">y, where <c>long.MinValue &lt;= x - y &lt;= long.MaxValue</c></param>
    /// <returns>The minimum of <paramref name="x"/> and <paramref name="y"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Min(long x, long y) =>
        y + ((x - y) & ((x - y) >> 63));

    /// <summary>
    /// Calculates the maximum of <paramref name="x"/> and <paramref name="y"/> where <c>int.MinValue &lt;= x - y &lt;= int.MaxValue</c>
    /// </summary>
    /// <param name="x">x, where <c>int.MinValue &lt;= x - y &lt;= int.MaxValue</c></param>
    /// <param name="y">y, where <c>int.MinValue &lt;= x - y &lt;= int.MaxValue</c></param>
    /// <returns>The maximum of <paramref name="x"/> and <paramref name="y"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Max(int x, int y) =>
        x - ((x - y) & ((x - y) >> 31));

    /// <summary>
    /// Calculates the maximum of <paramref name="x"/> and <paramref name="y"/> where <c>long.MinValue &lt;= x - y &lt;= long.MaxValue</c>
    /// </summary>
    /// <param name="x">x, where <c>long.MinValue &lt;= x - y &lt;= long.MaxValue</c></param>
    /// <param name="y">y, where <c>long.MinValue &lt;= x - y &lt;= long.MaxValue</c></param>
    /// <returns>The maximum of <paramref name="x"/> and <paramref name="y"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Max(long x, long y) =>
        x - ((x - y) & ((x - y) >> 63));
}