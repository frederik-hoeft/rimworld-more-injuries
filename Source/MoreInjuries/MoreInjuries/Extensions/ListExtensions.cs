using System.Collections.Generic;

namespace MoreInjuries.Extensions;

public static class ListExtensions
{
    private static readonly Random s_random = new();

    public static T SelectRandom<T>(this IReadOnlyList<T> list) => list[s_random.Next(list.Count)];
}
