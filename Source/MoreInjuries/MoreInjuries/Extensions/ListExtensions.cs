using System.Collections.Generic;

namespace MoreInjuries.Extensions;

public static class ListExtensions
{
    public static T SelectRandom<T>(this IReadOnlyList<T> list) => list[RandomX.Shared.Next(list.Count)];
}
