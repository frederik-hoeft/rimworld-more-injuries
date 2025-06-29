using System.Collections.Generic;

namespace MoreInjuries.Extensions;

internal static class SpanExtensions
{
    public static bool Contains<T>(this ReadOnlySpan<T> span, T value)
    {
        for (int i = 0; i < span.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(span[i], value))
            {
                return true;
            }
        }
        return false;
    }
}