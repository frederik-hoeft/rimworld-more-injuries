using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

/// <summary>
/// Checks type convertibility between Roslyn type symbols without requiring a Compilation.
/// </summary>
internal static class TypeConversions
{
    public static bool IsImplicitlyConvertible(ITypeSymbol source, ITypeSymbol target)
    {
        ITypeSymbol targetStripped = StripNullable(target);

        return source switch
        {
            _ when AreSameType(StripNullable(source), targetStripped) => true,
            _ when IsImplementedInterface(source, target, targetStripped) => true,
            _ => source.EnumerateBaseTypes()
                .Any(baseType => AreSameType(StripNullable(baseType), targetStripped)),
        };
    }

    private static ITypeSymbol StripNullable(ITypeSymbol symbol) =>
        symbol.WithNullableAnnotation(NullableAnnotation.None);

    private static bool AreSameType(ITypeSymbol source, ITypeSymbol target) =>
        SymbolEqualityComparer.Default.Equals(source, target);

    private static bool IsImplementedInterface(ITypeSymbol source, ITypeSymbol target, ITypeSymbol targetStripped) =>
        target.TypeKind is TypeKind.Interface
        && source.AllInterfaces.Any(candidate => AreSameType(StripNullable(candidate), targetStripped));
}
