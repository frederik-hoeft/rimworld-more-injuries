using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.Extensions;

namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization;

/// <summary>
/// Maps collection interface types to their concrete backing field types.
/// RimWorld's XML deserializer populates <c>List&lt;T&gt;</c> fields, so properties typed as
/// collection interfaces need their backing field to use the concrete type.
/// </summary>
internal static class CollectionTypeMapper
{
    private static readonly string[] s_supportedInterfaces =
    [
        "System.Collections.Generic.IReadOnlyList`1",
        "System.Collections.Generic.IReadOnlyCollection`1",
        "System.Collections.Generic.IList`1",
        "System.Collections.Generic.ICollection`1",
        "System.Collections.Generic.IEnumerable`1",
    ];

    /// <summary>
    /// If <paramref name="type"/> is a supported single-element generic collection interface,
    /// returns the fully-qualified <c>List&lt;T&gt;</c> type string. Otherwise returns <see langword="null"/>.
    /// </summary>
    public static string? TryGetConcreteType(ITypeSymbol type, SymbolDisplayFormat displayFormat)
    {
        ITypeSymbol strippedType = type.WithNullableAnnotation(NullableAnnotation.None);

        if (strippedType is not INamedTypeSymbol { IsGenericType: true, TypeArguments: [ITypeSymbol elementType] } namedType)
        {
            return null;
        }

        string metadataName = namedType.ConstructedFrom.GetFullMetadataName();
        foreach (string supported in s_supportedInterfaces)
        {
            if (metadataName.Equals(supported, StringComparison.Ordinal))
            {
                string elementTypeDisplay = elementType.ToDisplayString(displayFormat);
                return $"global::System.Collections.Generic.List<{elementTypeDisplay}>";
            }
        }

        return null;
    }
}
