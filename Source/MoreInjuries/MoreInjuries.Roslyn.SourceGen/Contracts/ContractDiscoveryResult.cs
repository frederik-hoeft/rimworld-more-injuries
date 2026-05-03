using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace MoreInjuries.Roslyn.SourceGen.Contracts;

internal sealed record ContractDiscoveryResult<TContract>(
    Dictionary<TContract, INamedTypeSymbol> Contracts,
    ImmutableArray<Diagnostic> Diagnostics)
    where TContract : unmanaged, Enum;
