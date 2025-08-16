using MoreInjuries.Roslyn.Future.ThrowHelpers;
using System.Collections.Generic;

namespace MoreInjuries.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<TResult> Transform<T, TResult>(this IEnumerable<T> enumerable, TransformationFunction<T, TResult> transform)
    {
        Throw.ArgumentNullException.IfNull(enumerable);
        Throw.ArgumentNullException.IfNull(transform);
        foreach (T item in enumerable)
        {
            if (transform(item, out TResult? result))
            {
                yield return result;
            }
        }
    }

    public delegate bool TransformationFunction<T, TResult>(T value, out TResult result);
}
