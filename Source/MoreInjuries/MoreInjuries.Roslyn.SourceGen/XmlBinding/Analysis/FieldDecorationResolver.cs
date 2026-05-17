using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal static class FieldDecorationResolver
{
    private static SymbolDisplayFormat FullyQualifiedFormat => SymbolDisplayFormats.s_fullyQualifiedWithNullable;

    public static BindingResult<FieldDecorationSpec> Resolve(
        AttributeData attribute,
        PropertyAnalysisContext context)
    {
        string? mayRequire = attribute.GetNamedStringArgument(nameof(XmlBindingAttribute.MayRequire));
        INamedTypeSymbol? optionalDecorateType = attribute.GetNamedTypeArgument(nameof(XmlBindingAttribute.DecorateWith));

        return optionalDecorateType is { } decorateType
            ? ValidateDecorateAttribute(context, decorateType).Map(display => new FieldDecorationSpec(display, mayRequire))
            : BindingResult<FieldDecorationSpec>.Success(new FieldDecorationSpec(DecorateAttributeDisplay: null, mayRequire));
    }

    private static BindingResult<string> ValidateDecorateAttribute(PropertyAnalysisContext context, INamedTypeSymbol decorateType) => decorateType switch
    {
        _ when FieldAttributeInspector.IsConcreteFieldAttribute(decorateType) => BindingResult<string>.Success(decorateType.ToDisplayString(FullyQualifiedFormat)),
        _ => BindingResult<string>.Failure(Diagnostic.Create(
            XmlSerializationGeneratorDiagnostics.InvalidDecorateAttribute,
            context.Location,
            context.PropertyName,
            context.TypeName,
            decorateType.ToDisplayString())),
    };
}
