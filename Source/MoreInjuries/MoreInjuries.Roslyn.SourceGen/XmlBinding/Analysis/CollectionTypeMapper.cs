using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

/// <summary>
/// Maps collection interface types to their concrete backing field types.
/// RimWorld's XML deserializer populates <c>List&lt;T&gt;</c> fields, so properties typed as
/// collection interfaces need their backing field to use the concrete type.
/// </summary>
internal static class CollectionTypeMapper
{
    private static readonly ImmutableHashSet<string> s_supportedInterfaces = ImmutableHashSet.CreateRange(
        StringComparer.Ordinal,
        [
            "System.Collections.Generic.IReadOnlyList`1",
            "System.Collections.Generic.IReadOnlyCollection`1",
            "System.Collections.Generic.IList`1",
            "System.Collections.Generic.ICollection`1",
            "System.Collections.Generic.IEnumerable`1",
        ]);

    /// <summary>
    /// If <paramref name="type"/> is a supported single-element generic collection interface,
    /// returns the fully-qualified <c>List&lt;T&gt;</c> type string. Otherwise returns <see langword="null"/>.
    /// </summary>
    public static string? TryGetConcreteType(ITypeSymbol type, SymbolDisplayFormat displayFormat) =>
        TryMap(type, displayFormat)?.Display;

    /// <summary>
    /// If <paramref name="type"/> is a supported single-element generic collection interface,
    /// returns the fully-qualified <c>List&lt;T&gt;</c> type string and the constructed <c>List&lt;T&gt;</c>
    /// type symbol (if resolvable). Otherwise returns <see langword="null"/>.
    /// </summary>
    public static string? TryGetConcreteType(ITypeSymbol type, SymbolDisplayFormat displayFormat, out INamedTypeSymbol? concreteTypeSymbol)
    {
        ConcreteCollectionMapping? mapping = TryMap(type, displayFormat);
        concreteTypeSymbol = mapping?.Symbol;
        return mapping?.Display;
    }

    private static ConcreteCollectionMapping? TryMap(ITypeSymbol type, SymbolDisplayFormat displayFormat) =>
        type.WithNullableAnnotation(NullableAnnotation.None) switch
        {
            INamedTypeSymbol { IsGenericType: true, TypeArguments: [ITypeSymbol elementType] } namedType
                when s_supportedInterfaces.Contains(namedType.ConstructedFrom.GetFullMetadataName()) =>
                    new ConcreteCollectionMapping(
                        $"global::System.Collections.Generic.List<{elementType.ToDisplayString(displayFormat)}>",
                        TryConstructListType(namedType, elementType)),
            _ => null,
        };

    private static INamedTypeSymbol? TryConstructListType(INamedTypeSymbol interfaceType, ITypeSymbol elementType) =>
        interfaceType.ConstructedFrom.ContainingAssembly
            .GetTypeByMetadataName("System.Collections.Generic.List`1")
            ?.Construct(elementType);
}
