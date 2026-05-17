using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

/// <summary>
/// Resolves and validates getter-pipeline method references from XML binding attributes.
/// </summary>
internal static class GetterMethodSymbolResolver
{
    private static SymbolDisplayFormat FullyQualifiedFormat => SymbolDisplayFormats.s_fullyQualifiedWithNullable;

    public static BindingResult<ResolvedMethod> ResolveValidateMethod(PropertyAnalysisContext context, string methodName) =>
        ResolveMethodBySignature(context, methodName, XmlSerializationGeneratorDiagnostics.ValidateMethodNotFound, static (method, propertyType) =>
            method.ReturnType.SpecialType == SpecialType.System_Boolean
            && propertyType.IsImplicitlyConvertible(method.Parameters[0].Type));

    public static BindingResult<ResolvedMethod> ResolveTransformMethod(PropertyAnalysisContext context, string methodName) =>
        ResolveMethodBySignature(context, methodName, XmlSerializationGeneratorDiagnostics.TransformMethodNotFound, static (method, propertyType) =>
            propertyType.IsImplicitlyConvertible(method.Parameters[0].Type) && method.ReturnType.IsImplicitlyConvertible(propertyType));

    private static BindingResult<ResolvedMethod> ResolveMethodBySignature(
        PropertyAnalysisContext context,
        string methodName,
        DiagnosticDescriptor notFoundDescriptor,
        Func<IMethodSymbol, ITypeSymbol, bool> signatureMatch)
    {
        ITypeSymbol propertyType = context.Property.Type;

        return context.TypeSymbol.GetMembers(methodName)
            .OfType<IMethodSymbol>()
            .Where(method => method.Parameters.Length == 1 && signatureMatch(method, propertyType))
            .Select(method => BindingResult<ResolvedMethod>.Success(new ResolvedMethod(methodName, method.IsStatic)))
            .FirstOrNull()
        ?? BindingResult<ResolvedMethod>.Failure(Diagnostic.Create(
            notFoundDescriptor,
            context.Location,
            context.PropertyName,
            context.TypeName,
            methodName,
            propertyType.ToDisplayString(FullyQualifiedFormat)));
    }
}
