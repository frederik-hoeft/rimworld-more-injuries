namespace MoreInjuries.Roslyn.SourceGen;

internal static class SymbolNameGenerator
{
    /// <summary>
    /// Generates a unique symbol name based on the provided name.
    /// </summary>
    public static string MakeUnique(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name), "Name must not be null or empty.");
        }
        return $"{name}_{Guid.NewGuid():N}__generated";
    }
}
