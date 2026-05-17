using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal static class XmlBindingModelFactory
{
    private static readonly (Func<IPropertySymbol, bool> Predicate, string Modifier)[] s_propertyModifiers =
    [
        (static property => property.IsStatic, "static"),
        (static property => property.IsVirtual, "virtual"),
        (static property => property is { IsSealed: true, IsOverride: true }, "sealed"),
        (static property => property.IsOverride, "override"),
        (static property => property.IsAbstract, "abstract"),
    ];

    private static SymbolDisplayFormat FullyQualifiedFormat => SymbolDisplayFormats.s_fullyQualifiedWithNullable;

    public static XmlBindingModel Create(
        IPropertySymbol property,
        ParsedBindingAttribute parsed,
        GetterPipelineSpec getterPipeline,
        FieldDecorationSpec decorations)
    {
        PropertyTypeSpec propertyType = PropertyTypeSpec.Create(property.Type, parsed.DefaultValue);
        string? concreteCollectionType = CollectionTypeMapper.TryGetConcreteType(property.Type, FullyQualifiedFormat);

        return new XmlBindingModel(
            Property: new XmlBindingPropertyModel(
                Name: property.Name,
                TypeDisplay: propertyType.PropertyDisplay,
                Accessibility: property.DeclaredAccessibility,
                Modifiers: GetPropertyModifiers(property),
                Setter: CreateSetterModel(property, concreteCollectionType)),
            Field: new XmlBindingFieldModel(
                Name: parsed.FieldName,
                TypeDisplay: GetFieldTypeDisplay(property.Type, propertyType, concreteCollectionType),
                DefaultValueExpression: parsed.DefaultValue.Expression,
                AllowRawAccess: parsed.AllowRawAccess,
                DecorateAttributeDisplay: decorations.DecorateAttributeDisplay,
                MayRequire: decorations.MayRequire),
            GetterPipeline: new GetterPipelineModel(
                RequiresNullCheck: propertyType.RequiresNullCheck,
                IsStringType: property.Type.SpecialType == SpecialType.System_String,
                IsValueTypeNullCheck: !propertyType.IsReferenceType
                    && !propertyType.IsNullableValueType
                    && parsed.DefaultValue.IsNullable,
                Transform: getterPipeline.Transform,
                Validate: getterPipeline.Validate));
    }

    private static SetterModel? CreateSetterModel(IPropertySymbol property, string? concreteCollectionType) =>
        property.SetMethod switch
        {
            null => null,
            { } setter => new SetterModel(
                setter.IsInitOnly,
                setter.DeclaredAccessibility,
                concreteCollectionType),
        };

    private static string GetFieldTypeDisplay(
        ITypeSymbol propertyType,
        PropertyTypeSpec propertyTypeSpec,
        string? concreteCollectionType) =>
        (concreteCollectionType, propertyTypeSpec.FieldIsNullable, propertyTypeSpec.IsReferenceType) switch
        {
            ({ } concreteType, true, _) => concreteType + "?",
            ({ } concreteType, false, _) => concreteType,
            (null, true, true) => propertyType.WithNullableAnnotation(NullableAnnotation.Annotated).ToDisplayString(FullyQualifiedFormat),
            (null, true, false) => propertyTypeSpec.PropertyDisplay + "?",
            _ => propertyTypeSpec.PropertyDisplay,
        };

    private static string GetPropertyModifiers(IPropertySymbol property) =>
        string.Join(" ", s_propertyModifiers
            .Where(modifier => modifier.Predicate(property))
            .Select(static modifier => modifier.Modifier)) switch
        {
            { Length: > 0 } modifiers => modifiers + " ",
            _ => string.Empty,
        };
}
