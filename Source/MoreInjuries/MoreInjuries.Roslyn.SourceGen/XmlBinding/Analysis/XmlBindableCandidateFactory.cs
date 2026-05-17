using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal static class XmlBindableCandidateFactory
{
    public static XmlBindableCandidate Create(XmlBindableTarget target) => ValidateTarget(target.TypeSymbol) switch
    {
        { IsSuccess: true, Value: { } typeSymbol } => CreateCandidate(typeSymbol),
        { } failure => new XmlBindableCandidate(null, failure.Diagnostics),
    };

    private static BindingResult<INamedTypeSymbol> ValidateTarget(INamedTypeSymbol typeSymbol) => typeSymbol switch
    {
        _ when typeSymbol.TypeKind is not TypeKind.Class => Failure(
            XmlSerializationGeneratorDiagnostics.TargetMustBeClass,
            typeSymbol,
            typeSymbol.Name),
        _ when !typeSymbol.HasPartialDeclaration() => Failure(
            XmlSerializationGeneratorDiagnostics.TargetTypeMustBePartial,
            typeSymbol,
            typeSymbol.Name),
        _ => BindingResult<INamedTypeSymbol>.Success(typeSymbol),
    };

    private static XmlBindableCandidate CreateCandidate(INamedTypeSymbol typeSymbol)
    {
        MemberAccumulator accumulator = typeSymbol.GetProperties().Aggregate(
            seed: MemberAccumulator.Empty,
            func: (state, property) => state.Analyze(new PropertyAnalysisContext(typeSymbol, property)));

        return new XmlBindableCandidate
        (
            new XmlBindableGenerationModel
            (
                Namespace: GetNamespaceName(typeSymbol),
                ClassName: typeSymbol.Name,
                AnnotatedMembers: [.. accumulator.Members]
            ),
            [.. accumulator.Diagnostics]
        );
    }

    private static string GetNamespaceName(INamedTypeSymbol typeSymbol) => typeSymbol.ContainingNamespace switch
    {
        { IsGlobalNamespace: false } namespaceSymbol => namespaceSymbol.ToDisplayString(),
        _ => string.Empty,
    };

    private static BindingResult<INamedTypeSymbol> Failure(DiagnosticDescriptor descriptor, INamedTypeSymbol typeSymbol, params object?[] messageArgs) =>
        BindingResult<INamedTypeSymbol>.Failure(Diagnostic.Create(descriptor, typeSymbol.Locations.FirstOrDefault(), messageArgs));
}
