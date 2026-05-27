using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

/// <summary>
/// Checks type convertibility between Roslyn type symbols without requiring a Compilation.
/// </summary>
internal static class TypeConversions
{
    extension(ITypeSymbol self)
    {
        public bool IsImplicitlyConvertible(ITypeSymbol target)
        {
            ITypeSymbol targetStripped = target.StripNullable();

            return self switch
            {
                _ when self.StripNullable().IsSameTypeAs(targetStripped) => true,
                _ when self.IsImplementedInterface(target, targetStripped) => true,
                _ => self.EnumerateBaseTypes()
                    .Any(baseType => baseType.StripNullable().IsSameTypeAs(targetStripped)),
            };
        }

        private ITypeSymbol StripNullable() => self.WithNullableAnnotation(NullableAnnotation.None);

        private bool IsSameTypeAs(ITypeSymbol other) => SymbolEqualityComparer.Default.Equals(self, other);

        private bool IsImplementedInterface(ITypeSymbol target, ITypeSymbol targetStripped) =>
            target.TypeKind is TypeKind.Interface
            && self.AllInterfaces.Any(candidate => candidate.StripNullable().IsSameTypeAs(targetStripped));
    }
}
