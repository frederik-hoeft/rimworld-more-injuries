using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding;

/// <summary>
/// Checks type convertibility between Roslyn type symbols without requiring a Compilation.
/// </summary>
internal static class TypeConversions
{
    public static bool IsImplicitlyConvertible(ITypeSymbol source, ITypeSymbol target)
    {
        ITypeSymbol sourceStripped = source.WithNullableAnnotation(NullableAnnotation.None);
        ITypeSymbol targetStripped = target.WithNullableAnnotation(NullableAnnotation.None);

        // Identity or nullable-widening conversion
        if (SymbolEqualityComparer.Default.Equals(sourceStripped, targetStripped))
        {
            return true;
        }

        // Interface implementation
        if (target.TypeKind is TypeKind.Interface)
        {
            return source.AllInterfaces.Any(i =>
                SymbolEqualityComparer.Default.Equals(i.WithNullableAnnotation(NullableAnnotation.None), targetStripped));
        }

        // Base type chain
        ITypeSymbol? current = source.BaseType;
        while (current is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(current.WithNullableAnnotation(NullableAnnotation.None), targetStripped))
            {
                return true;
            }
            current = current.BaseType;
        }

        return false;
    }
}
