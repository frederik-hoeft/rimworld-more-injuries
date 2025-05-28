using System.Collections.Generic;

namespace MoreInjuries.Extensions;

public static class ListExtensions
{
    public static T? SelectRandom<T>(this IReadOnlyList<T> list)
    {
        if (list.Count == 0)
        {
            return default;
        }
        return list[RandomX.Shared.Next(list.Count)];
    }
}
