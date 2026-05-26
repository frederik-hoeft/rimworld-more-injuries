using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

internal sealed record XmlBindableTarget(INamedTypeSymbol TypeSymbol, AttributeData GeneratorAttribute)
{
    public static XmlBindableTarget Create(GeneratorAttributeSyntaxContext context) =>
        new((INamedTypeSymbol)context.TargetSymbol, context.Attributes[0]);
}
