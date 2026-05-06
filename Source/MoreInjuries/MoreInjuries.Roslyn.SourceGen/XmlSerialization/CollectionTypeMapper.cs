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
    public static string? TryGetConcreteType(ITypeSymbol type, SymbolDisplayFormat displayFormat) =>
        TryGetConcreteType(type, displayFormat, out _);

    /// <summary>
    /// If <paramref name="type"/> is a supported single-element generic collection interface,
    /// returns the fully-qualified <c>List&lt;T&gt;</c> type string and the constructed <c>List&lt;T&gt;</c>
    /// type symbol (if resolvable). Otherwise returns <see langword="null"/>.
    /// </summary>
    public static string? TryGetConcreteType(ITypeSymbol type, SymbolDisplayFormat displayFormat, out INamedTypeSymbol? concreteTypeSymbol)
    {
        ITypeSymbol strippedType = type.WithNullableAnnotation(NullableAnnotation.None);

        if (strippedType is not INamedTypeSymbol { IsGenericType: true, TypeArguments: [ITypeSymbol elementType] } namedType)
        {
            concreteTypeSymbol = null;
            return null;
        }

        string metadataName = namedType.ConstructedFrom.GetFullMetadataName();
        foreach (string supported in s_supportedInterfaces)
        {
            if (metadataName.Equals(supported, StringComparison.Ordinal))
            {
                string elementTypeDisplay = elementType.ToDisplayString(displayFormat);
                concreteTypeSymbol = TryConstructListType(namedType, elementType);
                return $"global::System.Collections.Generic.List<{elementTypeDisplay}>";
            }
        }

        concreteTypeSymbol = null;
        return null;
    }

    private static INamedTypeSymbol? TryConstructListType(INamedTypeSymbol interfaceType, ITypeSymbol elementType)
    {
        // Resolve System.Collections.Generic.List`1 from the same assembly that defines the interface
        IAssemblySymbol containingAssembly = interfaceType.ConstructedFrom.ContainingAssembly;
        INamedTypeSymbol? listOpenType = containingAssembly.GetTypeByMetadataName("System.Collections.Generic.List`1");
        return listOpenType?.Construct(elementType);
    }
}
