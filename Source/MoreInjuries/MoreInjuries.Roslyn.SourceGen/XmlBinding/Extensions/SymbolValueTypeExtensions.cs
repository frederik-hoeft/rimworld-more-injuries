using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;

internal static class SymbolValueTypeExtensions
{
    extension(ISymbol symbol)
    {
        public ITypeSymbol GetValueType() => symbol switch
        {
            IFieldSymbol field => field.Type,
            IPropertySymbol property => property.Type,
            _ => throw new InvalidOperationException($"Unexpected symbol kind: {symbol.Kind}"),
        };
    }
}
