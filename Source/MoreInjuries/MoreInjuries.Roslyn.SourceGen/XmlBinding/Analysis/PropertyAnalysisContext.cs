using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal sealed record PropertyAnalysisContext(INamedTypeSymbol TypeSymbol, IPropertySymbol Property)
{
    public string TypeName => TypeSymbol.Name;

    public string PropertyName => Property.Name;

    public Location? Location => Property.Locations.FirstOrDefault();
}
