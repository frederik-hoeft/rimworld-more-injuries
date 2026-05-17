using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal sealed record DefaultValueSourceContext(
    PropertyAnalysisContext PropertyContext,
    ITypeSymbol TargetType,
    string SourceName)
{
    public INamedTypeSymbol TypeSymbol => PropertyContext.TypeSymbol;

    public IPropertySymbol Property => PropertyContext.Property;
}
