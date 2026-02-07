using MoreInjuries.Roslyn.Future.Extensions;
using System.Collections.Generic;

namespace MoreInjuries.Extensions.Bcl;

public static class ListExtensions
{
    public static T? SelectRandomOrDefault<T>(this IReadOnlyList<T> list)
    {
        if (list.Count == 0)
        {
            return default;
        }
        return list[Random.Shared.Next(list.Count)];
    }
}
