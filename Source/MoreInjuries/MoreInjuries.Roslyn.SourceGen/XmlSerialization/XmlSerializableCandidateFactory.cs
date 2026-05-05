using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MoreInjuries.Roslyn.SourceGen.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization;

internal static class XmlSerializableCandidateFactory
{
    private static readonly string s_xmlMemberGenericFullName = typeof(XmlMemberAttribute<>).FullName;

    private static SymbolDisplayFormat FullyQualifiedFormat => SymbolDisplayFormats.FullyQualifiedWithNullable;

    public static XmlSerializableCandidate Create(XmlSerialiableTarget target)
    {
        ImmutableArray<Diagnostic>.Builder diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
        INamedTypeSymbol typeSymbol = target.TypeSymbol;

        if (typeSymbol.TypeKind is not TypeKind.Class)
        {
            diagnostics.Add(Diagnostic.Create(
                XmlSerializationGeneratorDiagnostics.TargetMustBeClass,
                typeSymbol.Locations.FirstOrDefault(),
                typeSymbol.Name));

            return new XmlSerializableCandidate(null, diagnostics.ToImmutable());
        }

        ImmutableArray<XmlMemberModel>.Builder memberModels = ImmutableArray.CreateBuilder<XmlMemberModel>();

        foreach (IPropertySymbol property in typeSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            string? fieldName;
            string? defaultValueExpression = null;
            bool defaultValueIsNullable = false;
            bool allowRawAccess;
            AttributeData? resolvedAttribute;

            if (property.TryGetAttribute<XmlMemberAttribute>(out AttributeData? xmlMemberAttribute)
                && xmlMemberAttribute.ConstructorArguments is [{ Value: string nonGenericFieldName }])
            {
                fieldName = nonGenericFieldName;
                allowRawAccess = AttributeDataReader.GetAllowRawAccess(xmlMemberAttribute);
                resolvedAttribute = xmlMemberAttribute;

                // Resolve default value
                if (!TryResolveDefaultValue(xmlMemberAttribute, typeSymbol, property, diagnostics, out defaultValueExpression, out defaultValueIsNullable))
                {
                    continue;
                }
            }
            else if (!TryGetGenericXmlMemberAttribute(property, out fieldName, out defaultValueExpression, out allowRawAccess, out resolvedAttribute))
            {
                continue;
            }

            // Validate and Transform (shared for both attribute variants)
            if (!TryExtractGetterPipelineMethods(resolvedAttribute!, typeSymbol, property, diagnostics,
                out string? validateMethodName, out bool validateIsStatic,
                out string? transformMethodName, out bool transformIsStatic))
            {
                continue;
            }

            if (!property.IsPartialDefinition)
            {
                diagnostics.Add(Diagnostic.Create(
                    XmlSerializationGeneratorDiagnostics.MemberMustBePartial,
                    property.Locations.FirstOrDefault(),
                    property.Name,
                    typeSymbol.Name));
                continue;
            }
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                diagnostics.Add(Diagnostic.Create(
                    XmlSerializationGeneratorDiagnostics.InvalidFieldName,
                    property.Locations.FirstOrDefault(),
                    property.Name,
                    typeSymbol.Name,
                    fieldName));
                continue;
            }
            if (!typeSymbol.GetMembers(fieldName!).IsEmpty)
            {
                diagnostics.Add(Diagnostic.Create(
                    XmlSerializationGeneratorDiagnostics.MemberNameConflict,
                    property.Locations.FirstOrDefault(),
                    property.Name,
                    typeSymbol.Name,
                    fieldName));
                continue;
            }
            memberModels.Add(CreateMemberModel(property, fieldName!, defaultValueExpression, defaultValueIsNullable, allowRawAccess, validateMethodName, validateIsStatic, transformMethodName, transformIsStatic));
        }

        string namespaceName = typeSymbol.ContainingNamespace?.IsGlobalNamespace is false
            ? typeSymbol.ContainingNamespace.ToDisplayString()
            : string.Empty;

        return new XmlSerializableCandidate(
            new XmlSerializableGenerationModel(
                Namespace: namespaceName,
                ClassName: typeSymbol.Name,
                AnnotatedMembers: memberModels.ToImmutable()),
            diagnostics.ToImmutable());
    }

    private static XmlMemberModel CreateMemberModel(IPropertySymbol property, string fieldName, string? defaultValueExpression, bool defaultValueIsNullable, bool allowRawAccess, string? validateMethodName, bool validateIsStatic, string? transformMethodName, bool transformIsStatic)
    {
        bool hasSetter = property.SetMethod is not null;
        bool isInitOnly = property.SetMethod?.IsInitOnly ?? false;
        Accessibility setterAccessibility = property.SetMethod?.DeclaredAccessibility ?? Accessibility.NotApplicable;

        bool isReferenceType = property.Type.IsReferenceType;
        bool isNullableAnnotated = property.Type.NullableAnnotation == NullableAnnotation.Annotated;
        bool hasNonNullDefault = defaultValueExpression is not null && !defaultValueIsNullable;
        bool requiresNullCheck = isReferenceType && !isNullableAnnotated && !hasNonNullDefault;
        bool isStringType = property.Type.SpecialType == SpecialType.System_String;
        // The field must be nullable when it's initialized to 'default' (null) for reference types
        bool fieldIsNullable = requiresNullCheck || (isReferenceType && defaultValueExpression is null);

        string propertyTypeDisplay = property.Type.ToDisplayString(FullyQualifiedFormat);
        string propertyModifiers = GetPropertyModifiers(property);

        // Check if the property type is a collection interface that should bind to a concrete List<T>
        string? concreteCollectionType = CollectionTypeMapper.TryGetConcreteType(property.Type, FullyQualifiedFormat);
        string? setterCastType = hasSetter && concreteCollectionType is not null ? concreteCollectionType : null;

        string fieldTypeDisplay;
        if (concreteCollectionType is not null)
        {
            fieldTypeDisplay = fieldIsNullable ? concreteCollectionType + "?" : concreteCollectionType;
        }
        else
        {
            fieldTypeDisplay = fieldIsNullable
                ? property.Type.WithNullableAnnotation(NullableAnnotation.Annotated).ToDisplayString(FullyQualifiedFormat)
                : propertyTypeDisplay;
        }

        SetterModel? setter = hasSetter
            ? new SetterModel(isInitOnly, setterAccessibility, setterCastType)
            : null;

        GetterPipelineModel getterPipeline = new(
            RequiresNullCheck: requiresNullCheck,
            IsStringType: isStringType,
            Transform: transformMethodName is not null ? new MethodCallModel(transformMethodName, transformIsStatic) : null,
            Validate: validateMethodName is not null ? new MethodCallModel(validateMethodName, validateIsStatic) : null);

        return new XmlMemberModel(
            PropertyName: property.Name,
            PropertyTypeDisplay: propertyTypeDisplay,
            FieldTypeDisplay: fieldTypeDisplay,
            PropertyAccessibility: property.DeclaredAccessibility,
            PropertyModifiers: propertyModifiers,
            FieldName: fieldName,
            DefaultValueExpression: defaultValueExpression,
            AllowRawAccess: allowRawAccess,
            Setter: setter,
            GetterPipeline: getterPipeline);
    }

    private static string GetPropertyModifiers(IPropertySymbol property)
    {
        List<string> modifiers = [];
        if (property.IsVirtual) modifiers.Add("virtual");
        if (property.IsOverride) modifiers.Add("override");
        if (property.IsSealed && property.IsOverride) modifiers.Add("sealed");
        if (property.IsAbstract) modifiers.Add("abstract");
        if (property.IsStatic) modifiers.Add("static");
        return modifiers.Count > 0 ? string.Join(" ", modifiers) + " " : "";
    }

    private static bool TryGetGenericXmlMemberAttribute(
        IPropertySymbol property,
        out string? fieldName,
        out string? defaultValueExpression,
        out bool allowRawAccess,
        out AttributeData? attributeData)
    {
        foreach (AttributeData attribute in property.GetAttributes())
        {
            if (attribute.AttributeClass is { IsGenericType: true } attributeClass
                && attributeClass.ConstructUnboundGenericType().GetFullMetadataName()
                    .Equals(s_xmlMemberGenericFullName, StringComparison.Ordinal)
                && attribute.ConstructorArguments is [{ Value: string name }, TypedConstant defaultValue])
            {
                fieldName = name;
                defaultValueExpression = defaultValue.ToCSharpString();
                allowRawAccess = AttributeDataReader.GetAllowRawAccess(attribute);
                attributeData = attribute;
                return true;
            }
        }
        fieldName = null;
        defaultValueExpression = null;
        allowRawAccess = false;
        attributeData = null;
        return false;
    }

    /// <summary>
    /// Resolves the DefaultValueFrom/DefaultValueProvider combination from the non-generic attribute.
    /// Returns <see langword="false"/> if resolution fails (diagnostic already emitted).
    /// </summary>
    private static bool TryResolveDefaultValue(
        AttributeData xmlMemberAttribute,
        INamedTypeSymbol typeSymbol,
        IPropertySymbol property,
        ImmutableArray<Diagnostic>.Builder diagnostics,
        out string? defaultValueExpression,
        out bool defaultValueIsNullable)
    {
        INamedTypeSymbol? defaultValueProvider = AttributeDataReader.GetNamedTypeArgument(xmlMemberAttribute, nameof(XmlMemberAttribute.DefaultValueProvider));
        string? defaultValueFrom = AttributeDataReader.GetNamedStringArgument(xmlMemberAttribute, nameof(XmlMemberAttribute.DefaultValueFrom));

        if (defaultValueProvider is not null)
        {
            if (defaultValueFrom is null)
            {
                diagnostics.Add(Diagnostic.Create(
                    XmlSerializationGeneratorDiagnostics.DefaultValueProviderRequiresDefaultValueFrom,
                    property.Locations.FirstOrDefault(),
                    property.Name,
                    typeSymbol.Name));
                defaultValueExpression = null;
                defaultValueIsNullable = false;
                return false;
            }
            return MemberSymbolResolver.TryResolveDefaultValueFromProvider(defaultValueProvider, typeSymbol, property, defaultValueFrom, diagnostics, out defaultValueExpression, out defaultValueIsNullable);
        }

        if (defaultValueFrom is not null)
        {
            return MemberSymbolResolver.TryResolveDefaultValueFrom(typeSymbol, property, defaultValueFrom, diagnostics, out defaultValueExpression, out defaultValueIsNullable);
        }

        defaultValueExpression = null;
        defaultValueIsNullable = false;
        return true;
    }

    /// <summary>
    /// Extracts and resolves Validate/Transform method references from any <see cref="AttributeData"/>.
    /// Returns <see langword="false"/> if resolution fails (diagnostic already emitted).
    /// </summary>
    private static bool TryExtractGetterPipelineMethods(
        AttributeData attribute,
        INamedTypeSymbol typeSymbol,
        IPropertySymbol property,
        ImmutableArray<Diagnostic>.Builder diagnostics,
        out string? validateMethodName,
        out bool validateIsStatic,
        out string? transformMethodName,
        out bool transformIsStatic)
    {
        validateMethodName = null;
        validateIsStatic = false;
        transformMethodName = null;
        transformIsStatic = false;

        if (AttributeDataReader.GetNamedStringArgument(attribute, nameof(XmlMemberAttribute.Validate)) is { } validateName)
        {
            if (!MemberSymbolResolver.TryResolveValidateMethod(typeSymbol, property, validateName, diagnostics, out validateIsStatic))
            {
                return false;
            }
            validateMethodName = validateName;
        }

        if (AttributeDataReader.GetNamedStringArgument(attribute, nameof(XmlMemberAttribute.Transform)) is { } transformName)
        {
            if (!MemberSymbolResolver.TryResolveTransformMethod(typeSymbol, property, transformName, diagnostics, out transformIsStatic))
            {
                return false;
            }
            transformMethodName = transformName;
        }

        return true;
    }
}
