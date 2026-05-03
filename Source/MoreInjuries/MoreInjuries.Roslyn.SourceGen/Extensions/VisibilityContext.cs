using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.Extensions;

internal record VisibilityContext(Compilation Compilation, ISymbol VisibilityBoundary)
{
    public bool IsVisible(ISymbol other) => Compilation.IsSymbolAccessibleWithin(other, VisibilityBoundary);
}

internal sealed record VisibilityContext<T>(Compilation Compilation, T TypedVisibilityBoundary) : VisibilityContext(Compilation, TypedVisibilityBoundary) where T : ISymbol;
