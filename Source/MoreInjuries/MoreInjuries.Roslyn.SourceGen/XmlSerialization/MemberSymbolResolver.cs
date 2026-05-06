using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.Extensions;
using System;
using System.Collections.Immutable;

namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization;

/// <summary>
/// Resolves and validates symbol references from attribute arguments (default values, validate/transform methods).
/// </summary>
internal static class MemberSymbolResolver
{
    private static SymbolDisplayFormat FullyQualifiedFormat => SymbolDisplayFormats.FullyQualifiedWithNullable;

    public static bool TryResolveDefaultValueFrom(
        INamedTypeSymbol typeSymbol,
        IPropertySymbol property,
        ITypeSymbol targetType,
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
            return ValidateSourceType(typeSymbol, property, targetType, defaultValueFrom, parameter.Type, diagnostics, out defaultValueExpression, out isNullable);
        }

        // Next, try to find a static field or property with the given name
        ISymbol? staticMember = typeSymbol.GetMembers(defaultValueFrom)
            .FirstOrDefault(m => m.IsStatic && m is IFieldSymbol or IPropertySymbol);

        if (staticMember is not null)
        {
            ITypeSymbol memberType = GetMemberType(staticMember);
            return ValidateSourceType(typeSymbol, property, targetType, defaultValueFrom, memberType, diagnostics, out defaultValueExpression, out isNullable);
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

    public static bool TryResolveDefaultValueFromProvider(
        INamedTypeSymbol providerType,
        INamedTypeSymbol declaringType,
        IPropertySymbol property,
        ITypeSymbol targetType,
        string defaultValueFrom,
        ImmutableArray<Diagnostic>.Builder diagnostics,
        out string? defaultValueExpression,
        out bool isNullable)
    {
        ISymbol? staticMember = providerType.GetMembers(defaultValueFrom)
            .FirstOrDefault(m => m.IsStatic && m is IFieldSymbol or IPropertySymbol);

        if (staticMember is null)
        {
            diagnostics.Add(Diagnostic.Create(
                XmlSerializationGeneratorDiagnostics.DefaultValueProviderMemberNotFound,
                property.Locations.FirstOrDefault(),
                property.Name,
                declaringType.Name,
                providerType.ToDisplayString(FullyQualifiedFormat),
                defaultValueFrom));
            defaultValueExpression = null;
            isNullable = false;
            return false;
        }

        ITypeSymbol memberType = GetMemberType(staticMember);
        if (!TypeConversions.IsImplicitlyConvertible(memberType, targetType))
        {
            diagnostics.Add(Diagnostic.Create(
                XmlSerializationGeneratorDiagnostics.DefaultValueFromTypeMismatch,
                property.Locations.FirstOrDefault(),
                property.Name,
                declaringType.Name,
                defaultValueFrom,
                memberType.ToDisplayString(FullyQualifiedFormat),
                targetType.ToDisplayString(FullyQualifiedFormat)));
            defaultValueExpression = null;
            isNullable = false;
            return false;
        }

        isNullable = memberType.NullableAnnotation == NullableAnnotation.Annotated;
        defaultValueExpression = $"{providerType.ToDisplayString(FullyQualifiedFormat)}.{defaultValueFrom}";
        return true;
    }

    public static bool TryResolveValidateMethod(
        INamedTypeSymbol typeSymbol,
        IPropertySymbol property,
        string methodName,
        ImmutableArray<Diagnostic>.Builder diagnostics,
        out bool isStatic) =>
        TryResolveMethodBySignature(typeSymbol, property, methodName,
            static (method, propertyType) =>
                method.ReturnType.SpecialType == SpecialType.System_Boolean
                && TypeConversions.IsImplicitlyConvertible(propertyType, method.Parameters[0].Type),
            XmlSerializationGeneratorDiagnostics.ValidateMethodNotFound,
            diagnostics, out isStatic);

    public static bool TryResolveTransformMethod(
        INamedTypeSymbol typeSymbol,
        IPropertySymbol property,
        string methodName,
        ImmutableArray<Diagnostic>.Builder diagnostics,
        out bool isStatic) =>
        TryResolveMethodBySignature(typeSymbol, property, methodName,
            static (method, propertyType) =>
                TypeConversions.IsImplicitlyConvertible(propertyType, method.Parameters[0].Type)
                && TypeConversions.IsImplicitlyConvertible(method.ReturnType, propertyType),
            XmlSerializationGeneratorDiagnostics.TransformMethodNotFound,
            diagnostics, out isStatic);

    private static bool TryResolveMethodBySignature(
        INamedTypeSymbol typeSymbol,
        IPropertySymbol property,
        string methodName,
        Func<IMethodSymbol, ITypeSymbol, bool> signatureMatch,
        DiagnosticDescriptor notFoundDescriptor,
        ImmutableArray<Diagnostic>.Builder diagnostics,
        out bool isStatic)
    {
        ITypeSymbol propertyType = property.Type;
        foreach (IMethodSymbol method in typeSymbol.GetMembers(methodName).OfType<IMethodSymbol>())
        {
            if (method.Parameters.Length == 1 && signatureMatch(method, propertyType))
            {
                isStatic = method.IsStatic;
                return true;
            }
        }

        diagnostics.Add(Diagnostic.Create(
            notFoundDescriptor,
            property.Locations.FirstOrDefault(),
            property.Name,
            typeSymbol.Name,
            methodName,
            propertyType.ToDisplayString(FullyQualifiedFormat)));
        isStatic = false;
        return false;
    }

    private static bool ValidateSourceType(
        INamedTypeSymbol typeSymbol,
        IPropertySymbol property,
        ITypeSymbol targetType,
        string defaultValueFrom,
        ITypeSymbol sourceType,
        ImmutableArray<Diagnostic>.Builder diagnostics,
        out string? defaultValueExpression,
        out bool isNullable)
    {
        if (!TypeConversions.IsImplicitlyConvertible(sourceType, targetType))
        {
            diagnostics.Add(Diagnostic.Create(
                XmlSerializationGeneratorDiagnostics.DefaultValueFromTypeMismatch,
                property.Locations.FirstOrDefault(),
                property.Name,
                typeSymbol.Name,
                defaultValueFrom,
                sourceType.ToDisplayString(FullyQualifiedFormat),
                targetType.ToDisplayString(FullyQualifiedFormat)));
            defaultValueExpression = null;
            isNullable = false;
            return false;
        }

        isNullable = sourceType.NullableAnnotation == NullableAnnotation.Annotated;
        defaultValueExpression = defaultValueFrom;
        return true;
    }

    private static ITypeSymbol GetMemberType(ISymbol member) => member switch
    {
        IFieldSymbol field => field.Type,
        IPropertySymbol prop => prop.Type,
        _ => throw new InvalidOperationException($"Unexpected symbol kind: {member.Kind}")
    };
}
