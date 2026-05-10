using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MoreInjuries.Roslyn.SourceGen.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding;

internal static class XmlBindableCandidateFactory
{
    private static readonly string s_xmlMemberGenericFullName = typeof(XmlBindingAttribute<>).FullName;

    private static SymbolDisplayFormat FullyQualifiedFormat => SymbolDisplayFormats.FullyQualifiedWithNullable;

    public static XmlBindableCandidate Create(XmlBindableTarget target)
    {
        ImmutableArray<Diagnostic>.Builder diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
        INamedTypeSymbol typeSymbol = target.TypeSymbol;

        if (typeSymbol.TypeKind is not TypeKind.Class)
        {
            diagnostics.Add(Diagnostic.Create(
                XmlSerializationGeneratorDiagnostics.TargetMustBeClass,
                typeSymbol.Locations.FirstOrDefault(),
                typeSymbol.Name));

            return new XmlBindableCandidate(null, diagnostics.ToImmutable());
        }

        if (!typeSymbol.HasPartialDeclaration())
        {
            diagnostics.Add(Diagnostic.Create(
                XmlSerializationGeneratorDiagnostics.TargetTypeMustBePartial,
                typeSymbol.Locations.FirstOrDefault(),
                typeSymbol.Name));

            return new XmlBindableCandidate(null, diagnostics.ToImmutable());
        }

        ImmutableArray<XmlBindingModel>.Builder memberModels = ImmutableArray.CreateBuilder<XmlBindingModel>();
        HashSet<string> usedFieldNames = [];

        foreach (IPropertySymbol property in typeSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            string? fieldName;
            string? defaultValueExpression;
            bool defaultValueIsNullable;
            bool allowRawAccess;
            AttributeData? resolvedAttribute;

            if (property.TryGetAttribute<XmlBindingAttribute>(out AttributeData? xmlMemberAttribute)
                && xmlMemberAttribute.ConstructorArguments is [{ Value: string nonGenericFieldName }])
            {
                fieldName = nonGenericFieldName;
                allowRawAccess = AttributeDataReader.GetAllowRawAccess(xmlMemberAttribute);
                resolvedAttribute = xmlMemberAttribute;

                bool nullableBackingField = AttributeDataReader.GetNamedBoolArgument(xmlMemberAttribute, nameof(XmlBindingAttribute.NullableBackingField));
                if (nullableBackingField)
                {
                    defaultValueExpression = "null";
                    defaultValueIsNullable = true;
                }
                else if (!TryResolveDefaultValue(xmlMemberAttribute, typeSymbol, property, diagnostics, out defaultValueExpression, out defaultValueIsNullable))
                {
                    continue;
                }
            }
            else if (!TryGetGenericXmlMemberAttribute(property, out fieldName, out defaultValueExpression, out defaultValueIsNullable, out allowRawAccess, out resolvedAttribute))
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
            if (string.IsNullOrWhiteSpace(fieldName) || !SyntaxFacts.IsValidIdentifier(fieldName))
            {
                diagnostics.Add(Diagnostic.Create(
                    XmlSerializationGeneratorDiagnostics.InvalidFieldName,
                    property.Locations.FirstOrDefault(),
                    property.Name,
                    typeSymbol.Name,
                    fieldName));
                continue;
            }
            if (!typeSymbol.GetMembers(fieldName!).IsEmpty || !usedFieldNames.Add(fieldName!))
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

        return new XmlBindableCandidate(
            new XmlBindableGenerationModel(
                Namespace: namespaceName,
                ClassName: typeSymbol.Name,
                AnnotatedMembers: memberModels.ToImmutable()),
            diagnostics.ToImmutable());
    }

    private static XmlBindingModel CreateMemberModel(IPropertySymbol property, string fieldName, string? defaultValueExpression, bool defaultValueIsNullable, bool allowRawAccess, string? validateMethodName, bool validateIsStatic, string? transformMethodName, bool transformIsStatic)
    {
        bool hasSetter = property.SetMethod is not null;
        bool isInitOnly = property.SetMethod?.IsInitOnly ?? false;
        Accessibility setterAccessibility = property.SetMethod?.DeclaredAccessibility ?? Accessibility.NotApplicable;

        bool isReferenceType = property.Type.IsReferenceType;
        bool isNullableAnnotated = property.Type.NullableAnnotation == NullableAnnotation.Annotated;
        bool isNullableValueType = property.Type.IsValueType
            && property.Type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T };
        bool hasNonNullDefault = defaultValueExpression is not null && !defaultValueIsNullable;

        // Null check is needed when:
        // - Reference type that's non-nullable and has no non-null default, OR
        // - Non-nullable value type whose backing field is nullable (defaultValueIsNullable)
        bool requiresNullCheck =
            (isReferenceType && !isNullableAnnotated && !hasNonNullDefault)
            || (!isReferenceType && !isNullableValueType && defaultValueIsNullable);
        bool isValueTypeNullCheck = !isReferenceType && !isNullableValueType && defaultValueIsNullable;
        bool isStringType = property.Type.SpecialType == SpecialType.System_String;

        // The field must be nullable when:
        // - Reference type with null check, or reference type with no default value, OR
        // - Value type with NullableBackingField (defaultValueIsNullable for value types), OR
        // - Property type is already nullable value type (field matches property type naturally)
        bool fieldIsNullable = requiresNullCheck
            || (isReferenceType && defaultValueExpression is null)
            || (!isReferenceType && defaultValueIsNullable);

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
        else if (fieldIsNullable && isReferenceType)
        {
            fieldTypeDisplay = property.Type.WithNullableAnnotation(NullableAnnotation.Annotated).ToDisplayString(FullyQualifiedFormat);
        }
        else if (fieldIsNullable && !isReferenceType)
        {
            // For value types, append ? to produce Nullable<T> syntax
            fieldTypeDisplay = propertyTypeDisplay + "?";
        }
        else
        {
            fieldTypeDisplay = propertyTypeDisplay;
        }

        SetterModel? setter = hasSetter
            ? new SetterModel(isInitOnly, setterAccessibility, setterCastType)
            : null;

        GetterPipelineModel getterPipeline = new(
            RequiresNullCheck: requiresNullCheck,
            IsStringType: isStringType,
            IsValueTypeNullCheck: isValueTypeNullCheck,
            Transform: transformMethodName is not null ? new MethodCallModel(transformMethodName, transformIsStatic) : null,
            Validate: validateMethodName is not null ? new MethodCallModel(validateMethodName, validateIsStatic) : null);

        return new XmlBindingModel(
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
        ReadOnlySpan<(Predicate<IPropertySymbol> Predicate, string Modifier)> conditionalModifiers =
        [
            (static p => p.IsStatic, "static"),
            (static p => p.IsVirtual, "virtual"),
            (static p => p is { IsSealed: true, IsOverride: true }, "sealed"),
            (static p => p.IsOverride, "override"),
            (static p => p.IsAbstract, "abstract"),
        ];
        foreach ((Predicate<IPropertySymbol> predicate, string modifier) in conditionalModifiers)
        {
            if (predicate(property))
            {
                modifiers.Add(modifier);
            }
        }
        return modifiers.Count > 0 ? string.Join(" ", modifiers) + " " : string.Empty;
    }

    private static bool TryGetGenericXmlMemberAttribute(
        IPropertySymbol property,
        out string? fieldName,
        out string? defaultValueExpression,
        out bool defaultValueIsNullable,
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
                defaultValueExpression = defaultValue.ToCSharpStringWithPostfix();
                defaultValueIsNullable = false;
                allowRawAccess = AttributeDataReader.GetAllowRawAccess(attribute);
                attributeData = attribute;
                return true;
            }
        }
        fieldName = null;
        defaultValueExpression = null;
        defaultValueIsNullable = false;
        allowRawAccess = false;
        attributeData = null;
        return false;
    }

    /// <summary>
    /// Resolves the DefaultValueFrom/DefaultValueProvider combination from the non-generic attribute.
    /// Returns <see langword="false"/> if resolution fails (diagnostic already emitted).
    /// Validates against the backing field type (concrete collection type when applicable).
    /// </summary>
    private static bool TryResolveDefaultValue(
        AttributeData xmlMemberAttribute,
        INamedTypeSymbol typeSymbol,
        IPropertySymbol property,
        ImmutableArray<Diagnostic>.Builder diagnostics,
        out string? defaultValueExpression,
        out bool defaultValueIsNullable)
    {
        INamedTypeSymbol? defaultValueProvider = AttributeDataReader.GetNamedTypeArgument(xmlMemberAttribute, nameof(XmlBindingAttribute.DefaultValueProvider));
        string? defaultValueFrom = AttributeDataReader.GetNamedStringArgument(xmlMemberAttribute, nameof(XmlBindingAttribute.DefaultValueFrom));

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
            ITypeSymbol targetType = GetBackingFieldTargetType(property);
            return MemberSymbolResolver.TryResolveDefaultValueFromProvider(defaultValueProvider, typeSymbol, property, targetType, defaultValueFrom, diagnostics, out defaultValueExpression, out defaultValueIsNullable);
        }

        if (defaultValueFrom is not null)
        {
            ITypeSymbol targetType = GetBackingFieldTargetType(property);
            return MemberSymbolResolver.TryResolveDefaultValueFrom(typeSymbol, property, targetType, defaultValueFrom, diagnostics, out defaultValueExpression, out defaultValueIsNullable);
        }

        defaultValueExpression = null;
        defaultValueIsNullable = false;
        return true;
    }

    /// <summary>
    /// Returns the effective backing field type for a property: the concrete <c>List&lt;T&gt;</c> type symbol
    /// if the property is typed as a supported collection interface, otherwise the property type itself.
    /// </summary>
    private static ITypeSymbol GetBackingFieldTargetType(IPropertySymbol property)
    {
        CollectionTypeMapper.TryGetConcreteType(property.Type, FullyQualifiedFormat, out INamedTypeSymbol? concreteTypeSymbol);
        return concreteTypeSymbol ?? property.Type;
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

        if (AttributeDataReader.GetNamedStringArgument(attribute, nameof(XmlBindingAttribute.Validate)) is { } validateName)
        {
            if (!MemberSymbolResolver.TryResolveValidateMethod(typeSymbol, property, validateName, diagnostics, out validateIsStatic))
            {
                return false;
            }
            validateMethodName = validateName;
        }

        if (AttributeDataReader.GetNamedStringArgument(attribute, nameof(XmlBindingAttribute.Transform)) is { } transformName)
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
