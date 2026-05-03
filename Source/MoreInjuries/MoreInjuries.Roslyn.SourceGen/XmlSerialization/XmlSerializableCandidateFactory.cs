using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MoreInjuries.Roslyn.SourceGen.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Immutable;

namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization;

internal static class XmlSerializableCandidateFactory
{
    private static readonly string s_xmlMemberGenericFullName = typeof(XmlMemberAttribute<>).FullName;

    private static readonly SymbolDisplayFormat s_fullyQualifiedFormat =
        SymbolDisplayFormat.FullyQualifiedFormat
            .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Included)
            .WithMiscellaneousOptions(
                SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions
                | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

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

            if (property.TryGetAttribute<XmlMemberAttribute>(out AttributeData? xmlMemberAttribute)
                && xmlMemberAttribute.ConstructorArguments is [{ Value: string nonGenericFieldName }])
            {
                fieldName = nonGenericFieldName;
            }
            else if (!TryGetGenericXmlMemberAttribute(property, out fieldName, out defaultValueExpression))
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
            memberModels.Add(CreateMemberModel(property, fieldName!, defaultValueExpression));
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

    private static XmlMemberModel CreateMemberModel(IPropertySymbol property, string fieldName, string? defaultValueExpression)
    {
        bool hasSetter = property.SetMethod is not null;
        bool isInitOnly = property.SetMethod?.IsInitOnly ?? false;
        Accessibility setterAccessibility = property.SetMethod?.DeclaredAccessibility ?? Accessibility.NotApplicable;

        bool isReferenceType = property.Type.IsReferenceType;
        bool isNullableAnnotated = property.Type.NullableAnnotation == NullableAnnotation.Annotated;
        bool hasDefaultValue = defaultValueExpression is not null;
        bool requiresNullCheck = isReferenceType && !isNullableAnnotated && !hasDefaultValue;
        bool isStringType = property.Type.SpecialType == SpecialType.System_String;

        string propertyTypeDisplay = property.Type.ToDisplayString(s_fullyQualifiedFormat);
        string fieldTypeDisplay = requiresNullCheck
            ? property.Type.WithNullableAnnotation(NullableAnnotation.Annotated).ToDisplayString(s_fullyQualifiedFormat)
            : propertyTypeDisplay;

        return new XmlMemberModel(
            PropertyName: property.Name,
            PropertyTypeDisplay: propertyTypeDisplay,
            FieldTypeDisplay: fieldTypeDisplay,
            PropertyAccessibility: property.DeclaredAccessibility,
            FieldName: fieldName,
            DefaultValueExpression: defaultValueExpression,
            HasSetter: hasSetter,
            IsInitOnly: isInitOnly,
            SetterAccessibility: setterAccessibility,
            RequiresNullCheck: requiresNullCheck,
            IsStringType: isStringType);
    }

    private static bool TryGetGenericXmlMemberAttribute(
        IPropertySymbol property,
        out string? fieldName,
        out string? defaultValueExpression)
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
                return true;
            }
        }
        fieldName = null;
        defaultValueExpression = null;
        return false;
    }
}
