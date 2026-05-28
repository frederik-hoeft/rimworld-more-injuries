using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal static class XmlBindingAttributeParser
{
    private static readonly string s_xmlMemberGenericFullName = typeof(XmlBindingAttribute<>).FullName;

    extension(PropertyAnalysisContext self)
    {
        public BindingResult<ParsedBindingAttribute>? TryParse() =>
            TryParseNonGeneric(self) ?? TryParseGeneric(self);
    }

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

    private static BindingResult<ParsedBindingAttribute>? TryParseGeneric(PropertyAnalysisContext context) => context.Property.GetAttributes()
        .Select(attribute => TryParseGeneric(context, attribute))
        .FirstOrDefault(static result => result is not null);

    private static BindingResult<ParsedBindingAttribute>? TryParseGeneric(PropertyAnalysisContext context, AttributeData attribute) => attribute switch
    {
        {
            AttributeClass: { IsGenericType: true } attributeClass,
            ConstructorArguments: [{ Value: string fieldName }, TypedConstant defaultValue],
        } when attributeClass.ConstructUnboundGenericType().GetFullMetadataName().Equals(s_xmlMemberGenericFullName, StringComparison.Ordinal) =>
            ValidateGenericTypeArgument(context, attributeClass, fieldName, defaultValue, attribute),
        _ => null,
    };

    private static BindingResult<ParsedBindingAttribute> ValidateGenericTypeArgument(
        PropertyAnalysisContext context,
        INamedTypeSymbol attributeClass,
        string fieldName,
        TypedConstant defaultValue,
        AttributeData attribute)
    {
        ITypeSymbol genericTypeArg = attributeClass.TypeArguments[0];
        ITypeSymbol propertyType = context.Property.Type;

        if (!genericTypeArg.IsImplicitlyConvertible(propertyType))
        {
            return BindingResult<ParsedBindingAttribute>.Failure(Diagnostic.Create(
                XmlSerializationGeneratorDiagnostics.GenericDefaultValueTypeMismatch,
                context.Location,
                context.PropertyName,
                context.TypeName,
                genericTypeArg.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                propertyType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
        }

        return BindingResult<ParsedBindingAttribute>.Success(new ParsedBindingAttribute(
            fieldName,
            DefaultValue: new DefaultValueSpec(defaultValue.ToCSharpStringWithPostfix(), IsNullable: false),
            attribute.GetAllowRawAccess(),
            attribute));
    }
}
