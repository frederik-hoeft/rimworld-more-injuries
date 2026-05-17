using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;

internal static class SymbolTraversalExtensions
{
    extension(INamedTypeSymbol type)
    {
        public IEnumerable<INamedTypeSymbol> EnumerateBaseTypes()
        {
            for (INamedTypeSymbol? current = type.BaseType; current is not null; current = current.BaseType)
            {
                yield return current;
            }
        }

        public IEnumerable<IPropertySymbol> GetProperties() =>
            type.GetMembers().OfType<IPropertySymbol>();
    }

    extension(ITypeSymbol type)
    {
        public IEnumerable<ITypeSymbol> EnumerateBaseTypes()
        {
            for (ITypeSymbol? current = type.BaseType; current is not null; current = current.BaseType)
            {
                yield return current;
            }
        }
    }
}
