using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization;

internal sealed record XmlSerialiableTarget(INamedTypeSymbol TypeSymbol, AttributeData GeneratorAttribute)
{
    public static XmlSerialiableTarget Create(GeneratorAttributeSyntaxContext context) =>
        new((INamedTypeSymbol)context.TargetSymbol, context.Attributes[0]);
}
