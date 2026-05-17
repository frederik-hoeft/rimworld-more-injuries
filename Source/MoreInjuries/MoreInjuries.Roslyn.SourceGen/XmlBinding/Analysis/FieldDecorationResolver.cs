using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal static class FieldDecorationResolver
{
    extension (PropertyAnalysisContext self)
    {
        public BindingResult<FieldDecorationSpec> ResolveFieldDecorations(ParsedBindingAttribute binding)
        {
            AttributeData attribute = binding.Attribute;
            string? mayRequire = attribute.GetNamedStringArgument(nameof(XmlBindingAttribute.MayRequire));
            INamedTypeSymbol? optionalDecorateType = attribute.GetNamedTypeArgument(nameof(XmlBindingAttribute.DecorateWith));

            return optionalDecorateType is { } decorateType
                ? self.ValidateDecorateAttribute(decorateType).Map(display => new FieldDecorationSpec(binding, display, mayRequire))
                : BindingResult<FieldDecorationSpec>.Success(new FieldDecorationSpec(binding, DecorateAttributeDisplay: null, mayRequire));
        }

        private BindingResult<string> ValidateDecorateAttribute(INamedTypeSymbol decorateType) => decorateType switch
        {
            _ when FieldAttributeInspector.IsConcreteFieldAttribute(decorateType) => BindingResult<string>.Success(decorateType.ToDisplayString(SymbolDisplayFormats.FullyQualifiedWithNullable)),
            _ => BindingResult<string>.Failure(Diagnostic.Create(
                XmlSerializationGeneratorDiagnostics.InvalidDecorateAttribute,
                self.Location,
                self.PropertyName,
                self.TypeName,
                decorateType.ToDisplayString())),
        };
    }
}
