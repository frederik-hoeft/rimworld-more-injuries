using System.Collections.Immutable;

namespace MoreInjuries.LocalizationTests.Model;

internal static class Options
{
    public static IReadOnlyDictionary<string, bool> Empty { get; } = new Dictionary<string, bool>(capacity: 1).ToImmutableDictionary();
}
