using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding;

/// <summary>
/// Shared <see cref="SymbolDisplayFormat"/> instances used across the XmlSerialization pipeline.
/// </summary>
internal static class SymbolDisplayFormats
{
    /// <summary>
    /// Fully-qualified format including global:: prefix and nullable reference type annotations.
    /// </summary>
    public static readonly SymbolDisplayFormat FullyQualifiedWithNullable =
        SymbolDisplayFormat.FullyQualifiedFormat
            .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Included)
            .WithMiscellaneousOptions(
                SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions
                | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
}
