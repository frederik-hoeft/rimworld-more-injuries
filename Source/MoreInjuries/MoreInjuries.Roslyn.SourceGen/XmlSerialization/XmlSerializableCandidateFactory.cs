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
            bool defaultValueIsNullable = false;
            bool allowRawAccess;

            if (property.TryGetAttribute<XmlMemberAttribute>(out AttributeData? xmlMemberAttribute)
                && xmlMemberAttribute.ConstructorArguments is [{ Value: string nonGenericFieldName }])
            {
                fieldName = nonGenericFieldName;
                allowRawAccess = GetAllowRawAccess(xmlMemberAttribute);

                // Check for DefaultValueFrom named argument
                if (GetNamedStringArgument(xmlMemberAttribute, nameof(XmlMemberAttribute.DefaultValueFrom)) is { } defaultValueFrom)
                {
                    if (!TryResolveDefaultValueFrom(typeSymbol, property, defaultValueFrom, diagnostics, out defaultValueExpression, out defaultValueIsNullable))
                    {
                        continue;
                    }
                }
            }
            else if (!TryGetGenericXmlMemberAttribute(property, out fieldName, out defaultValueExpression, out allowRawAccess))
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
            memberModels.Add(CreateMemberModel(property, fieldName!, defaultValueExpression, defaultValueIsNullable, allowRawAccess));
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

    private static XmlMemberModel CreateMemberModel(IPropertySymbol property, string fieldName, string? defaultValueExpression, bool defaultValueIsNullable, bool allowRawAccess)
    {
        bool hasSetter = property.SetMethod is not null;
        bool isInitOnly = property.SetMethod?.IsInitOnly ?? false;
        Accessibility setterAccessibility = property.SetMethod?.DeclaredAccessibility ?? Accessibility.NotApplicable;

        bool isReferenceType = property.Type.IsReferenceType;
        bool isNullableAnnotated = property.Type.NullableAnnotation == NullableAnnotation.Annotated;
        bool hasNonNullDefault = defaultValueExpression is not null && !defaultValueIsNullable;
        bool requiresNullCheck = isReferenceType && !isNullableAnnotated && !hasNonNullDefault;
        bool isStringType = property.Type.SpecialType == SpecialType.System_String;

        string propertyTypeDisplay = property.Type.ToDisplayString(s_fullyQualifiedFormat);

        // Check if the property type is a collection interface that should bind to a concrete List<T>
        string? concreteCollectionType = TryGetConcreteCollectionType(property.Type);
        string? setterCastType = null;

        string fieldTypeDisplay;
        if (concreteCollectionType is not null)
        {
            fieldTypeDisplay = requiresNullCheck ? concreteCollectionType + "?" : concreteCollectionType;
            if (hasSetter)
            {
                setterCastType = concreteCollectionType;
            }
        }
        else
        {
            fieldTypeDisplay = requiresNullCheck
                ? property.Type.WithNullableAnnotation(NullableAnnotation.Annotated).ToDisplayString(s_fullyQualifiedFormat)
                : propertyTypeDisplay;
        }

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
            IsStringType: isStringType,
            AllowRawAccess: allowRawAccess,
            SetterCastType: setterCastType);
    }

    private static string? TryGetConcreteCollectionType(ITypeSymbol type)
    {
        // Strip nullable annotation for interface check
        ITypeSymbol strippedType = type.WithNullableAnnotation(NullableAnnotation.None);

        if (strippedType is INamedTypeSymbol { IsGenericType: true, TypeArguments: [ITypeSymbol elementType] } namedType)
        {
            string metadataName = namedType.ConstructedFrom.GetFullMetadataName();
            if (metadataName is "System.Collections.Generic.IReadOnlyList`1"
                or "System.Collections.Generic.IReadOnlyCollection`1"
                or "System.Collections.Generic.IList`1"
                or "System.Collections.Generic.ICollection`1"
                or "System.Collections.Generic.IEnumerable`1")
            {
                string elementTypeDisplay = elementType.ToDisplayString(s_fullyQualifiedFormat);
                return $"global::System.Collections.Generic.List<{elementTypeDisplay}>";
            }
        }

        return null;
    }

    private static bool TryGetGenericXmlMemberAttribute(
        IPropertySymbol property,
        out string? fieldName,
        out string? defaultValueExpression,
        out bool allowRawAccess)
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
                allowRawAccess = GetAllowRawAccess(attribute);
                return true;
            }
        }
        fieldName = null;
        defaultValueExpression = null;
        allowRawAccess = false;
        return false;
    }

    private static bool GetAllowRawAccess(AttributeData attribute)
    {
        foreach (KeyValuePair<string, TypedConstant> namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == nameof(XmlMemberAttribute.AllowRawAccess)
                && namedArg.Value.Value is true)
            {
                return true;
            }
        }
        return false;
    }

    private static string? GetNamedStringArgument(AttributeData attribute, string argumentName)
    {
        foreach (KeyValuePair<string, TypedConstant> namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == argumentName && namedArg.Value.Value is string { Length: > 0 } value)
            {
                return value;
            }
        }
        return null;
    }

    private static bool TryResolveDefaultValueFrom(
        INamedTypeSymbol typeSymbol,
        IPropertySymbol property,
        string defaultValueFrom,
        ImmutableArray<Diagnostic>.Builder diagnostics,
        out string? defaultValueExpression,
        out bool isNullable)
    {
        // First, try to find a primary constructor parameter with the given name
        IMethodSymbol? primaryCtor = typeSymbol.InstanceConstructors
            .FirstOrDefault(c => c.DeclaringSyntaxReferences
                .Any(r => r.GetSyntax() is Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax));

        IParameterSymbol? parameter = primaryCtor?.Parameters
            .FirstOrDefault(p => p.Name.Equals(defaultValueFrom, StringComparison.Ordinal));

        if (parameter is not null)
        {
            return ValidateSourceType(typeSymbol, property, defaultValueFrom, parameter.Type, diagnostics, out defaultValueExpression, out isNullable);
        }

        // Next, try to find a static field or property with the given name
        ISymbol? staticMember = typeSymbol.GetMembers(defaultValueFrom)
            .FirstOrDefault(m => m.IsStatic && m is IFieldSymbol or IPropertySymbol);

        if (staticMember is not null)
        {
            ITypeSymbol memberType = staticMember switch
            {
                IFieldSymbol field => field.Type,
                IPropertySymbol prop => prop.Type,
                _ => throw new InvalidOperationException()
            };
            return ValidateSourceType(typeSymbol, property, defaultValueFrom, memberType, diagnostics, out defaultValueExpression, out isNullable);
        }

        diagnostics.Add(Diagnostic.Create(
            XmlSerializationGeneratorDiagnostics.DefaultValueFromMemberNotFound,
            property.Locations.FirstOrDefault(),
            property.Name,
            typeSymbol.Name,
            defaultValueFrom));
        defaultValueExpression = null;
        isNullable = false;
        return false;
    }

    private static bool ValidateSourceType(
        INamedTypeSymbol typeSymbol,
        IPropertySymbol property,
        string defaultValueFrom,
        ITypeSymbol sourceType,
        ImmutableArray<Diagnostic>.Builder diagnostics,
        out string? defaultValueExpression,
        out bool isNullable)
    {
        ITypeSymbol targetType = property.Type;
        if (!IsImplicitlyConvertible(sourceType, targetType))
        {
            diagnostics.Add(Diagnostic.Create(
                XmlSerializationGeneratorDiagnostics.DefaultValueFromTypeMismatch,
                property.Locations.FirstOrDefault(),
                property.Name,
                typeSymbol.Name,
                defaultValueFrom,
                sourceType.ToDisplayString(s_fullyQualifiedFormat),
                targetType.ToDisplayString(s_fullyQualifiedFormat)));
            defaultValueExpression = null;
            isNullable = false;
            return false;
        }

        isNullable = sourceType.NullableAnnotation == NullableAnnotation.Annotated;
        defaultValueExpression = defaultValueFrom;
        return true;
    }

    private static bool IsImplicitlyConvertible(ITypeSymbol source, ITypeSymbol target)
    {
        // Strip nullable annotations for structural comparison
        ITypeSymbol sourceStripped = source.WithNullableAnnotation(NullableAnnotation.None);
        ITypeSymbol targetStripped = target.WithNullableAnnotation(NullableAnnotation.None);

        // Identity or nullable-widening conversion
        if (SymbolEqualityComparer.Default.Equals(sourceStripped, targetStripped))
        {
            return true;
        }

        // Interface implementation
        if (target.TypeKind is TypeKind.Interface)
        {
            return source.AllInterfaces.Any(i =>
                SymbolEqualityComparer.Default.Equals(i.WithNullableAnnotation(NullableAnnotation.None), targetStripped));
        }

        // Base type chain
        ITypeSymbol? current = source.BaseType;
        while (current is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(current.WithNullableAnnotation(NullableAnnotation.None), targetStripped))
            {
                return true;
            }
            current = current.BaseType;
        }

        return false;
    }
}
