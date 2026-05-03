namespace MoreInjuries.Roslyn.SourceGen.Extensions;

internal static class SymbolNameGenerator
{
    /// <summary>
    /// Generates a unique symbol name based on the provided name.
    /// </summary>
    public static string MakeUnique(string name) => name switch
    {
        not { Length: > 0 } => $"Local_{Guid.NewGuid():N}__generated",
        _ => $"{name}_{Guid.NewGuid():N}__generated",
    };

    public static string MakeCamelCase(string name) => name switch
    {
        not { Length: > 0 } => string.Empty,
        { Length: 1 } => name.ToLowerInvariant(),
        _ => $"{char.ToLowerInvariant(name[0])}{name[1..]}",
    };
}
