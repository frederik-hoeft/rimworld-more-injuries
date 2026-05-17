using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal static class XmlBindingAttributeParser
{
    private static readonly string s_xmlMemberGenericFullName = typeof(XmlBindingAttribute<>).FullName;

    public static BindingResult<ParsedBindingAttribute>? TryParse(PropertyAnalysisContext context) =>
        TryParseNonGeneric(context)
        ?? TryParseGeneric(context.Property);

    private static BindingResult<ParsedBindingAttribute>? TryParseNonGeneric(PropertyAnalysisContext context)
    {
        if (!context.Property.TryGetAttribute<XmlBindingAttribute>(out AttributeData? attribute)
            || attribute.ConstructorArguments is not [{ Value: string fieldName }])
        {
            return null;
        }

        bool allowRawAccess = attribute.GetAllowRawAccess();

        return attribute.GetNamedBoolArgument(nameof(XmlBindingAttribute.NullableBackingField)) switch
        {
            true => BindingResult<ParsedBindingAttribute>.Success(new ParsedBindingAttribute(
                fieldName,
                DefaultValue: new DefaultValueSpec("null", IsNullable: true),
                allowRawAccess,
                attribute)),
            false => DefaultValueResolver.Resolve(attribute, context)
                .Map(defaultValue => new ParsedBindingAttribute(fieldName, defaultValue, allowRawAccess, attribute)),
        };
    }

    private static BindingResult<ParsedBindingAttribute>? TryParseGeneric(IPropertySymbol property) =>
        property.GetAttributes()
            .Select(TryParseGeneric)
            .FirstOrDefault(static result => result is not null);

    private static BindingResult<ParsedBindingAttribute>? TryParseGeneric(AttributeData attribute) =>
        attribute switch
        {
            {
                AttributeClass: { IsGenericType: true } attributeClass,
                ConstructorArguments: [{ Value: string fieldName }, TypedConstant defaultValue],
            } when attributeClass.ConstructUnboundGenericType()
                .GetFullMetadataName()
                .Equals(s_xmlMemberGenericFullName, StringComparison.Ordinal) =>
                BindingResult<ParsedBindingAttribute>.Success(new ParsedBindingAttribute(
                    fieldName,
                    DefaultValue: new DefaultValueSpec(defaultValue.ToCSharpStringWithPostfix(), IsNullable: false),
                    attribute.GetAllowRawAccess(),
                    attribute)),
            _ => null,
        };
}
