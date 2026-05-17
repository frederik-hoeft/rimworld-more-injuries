using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

/// <summary>
/// Resolves and validates default-value member references from XML binding attributes.
/// </summary>
internal static class DefaultValueSymbolResolver
{
    private static SymbolDisplayFormat FullyQualifiedFormat => SymbolDisplayFormats.s_fullyQualifiedWithNullable;

    public static BindingResult<DefaultValueResolution> ResolveFromDeclaringType(DefaultValueSourceContext context) =>
        ResolvePrimaryConstructorParameter(context.TypeSymbol, context.SourceName)
            .Select(parameter => ValidateSourceType(context, parameter.Type, context.SourceName))
            .FirstOrNull()
        ?? ResolveStaticValueMember(context.TypeSymbol, context.SourceName)
            .Select(member => ValidateSourceType(context, member.GetValueType(), context.SourceName))
            .FirstOrNull()
        ?? BindingResult<DefaultValueResolution>.Failure(Diagnostic.Create(
            XmlSerializationGeneratorDiagnostics.DefaultValueFromMemberNotFound,
            context.PropertyContext.Location,
            context.PropertyContext.PropertyName,
            context.PropertyContext.TypeName,
            context.SourceName));

    public static BindingResult<DefaultValueResolution> ResolveFromProvider(
        INamedTypeSymbol providerType,
        DefaultValueSourceContext context) =>
        ResolveStaticValueMember(providerType, context.SourceName)
            .Select(member => ValidateProviderSourceType(providerType, context, member.GetValueType()))
            .FirstOrNull()
        ?? BindingResult<DefaultValueResolution>.Failure(Diagnostic.Create(
            XmlSerializationGeneratorDiagnostics.DefaultValueProviderMemberNotFound,
            context.PropertyContext.Location,
            context.PropertyContext.PropertyName,
            context.PropertyContext.TypeName,
            providerType.ToDisplayString(FullyQualifiedFormat),
            context.SourceName));

    private static IEnumerable<IParameterSymbol> ResolvePrimaryConstructorParameter(INamedTypeSymbol typeSymbol, string name) =>
        typeSymbol.InstanceConstructors
            .FirstOrDefault(IsPrimaryConstructorLike)
            ?.Parameters
            .Where(parameter => parameter.Name.Equals(name, StringComparison.Ordinal))
        ?? [];

    private static bool IsPrimaryConstructorLike(IMethodSymbol constructor) =>
        constructor.DeclaringSyntaxReferences.Any(static reference =>
            reference.GetSyntax() is Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax);

    private static IEnumerable<ISymbol> ResolveStaticValueMember(INamedTypeSymbol typeSymbol, string name) =>
        typeSymbol.GetMembers(name).Where(static member => member.IsStatic && member is IFieldSymbol or IPropertySymbol);

    private static BindingResult<DefaultValueResolution> ValidateProviderSourceType(
        INamedTypeSymbol providerType,
        DefaultValueSourceContext context,
        ITypeSymbol sourceType) =>
        ValidateSourceType(
            context,
            sourceType,
            $"{providerType.ToDisplayString(FullyQualifiedFormat)}.{context.SourceName}");

    private static BindingResult<DefaultValueResolution> ValidateSourceType(
        DefaultValueSourceContext context,
        ITypeSymbol sourceType,
        string defaultValueExpression) =>
        TypeConversions.IsImplicitlyConvertible(sourceType, context.TargetType)
            ? BindingResult<DefaultValueResolution>.Success(new DefaultValueResolution(
                defaultValueExpression,
                sourceType.NullableAnnotation == NullableAnnotation.Annotated))
            : BindingResult<DefaultValueResolution>.Failure(Diagnostic.Create(
                XmlSerializationGeneratorDiagnostics.DefaultValueFromTypeMismatch,
                context.PropertyContext.Location,
                context.PropertyContext.PropertyName,
                context.PropertyContext.TypeName,
                context.SourceName,
                sourceType.ToDisplayString(FullyQualifiedFormat),
                context.TargetType.ToDisplayString(FullyQualifiedFormat)));
}
