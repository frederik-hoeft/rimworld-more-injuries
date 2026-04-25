using Microsoft.CodeAnalysis;
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace MoreInjuries.Roslyn.SourceGen.Contracts;

internal abstract class ContractInfoBase<TContract>(Dictionary<TContract, INamedTypeSymbol> contractTypes, Compilation compilation) where TContract : unmanaged, Enum
{
    protected FrozenDictionary<TContract, INamedTypeSymbol> ContractTypes { get; } = contractTypes.ToFrozenDictionary();

    public Compilation Compilation => compilation;

    protected static Dictionary<TContract, INamedTypeSymbol>? FetchContracts(
        ContractExplorer contractExplorer,
        SourceProductionContext context,
        ImmutableArray<TContract> requiredContracts)
    {
        ContractDiscoveryResult<TContract> discovery = contractExplorer.DiscoverContracts<TContract>();

        foreach (Diagnostic diagnostic in discovery.Diagnostics)
        {
            context.ReportDiagnostic(diagnostic);
        }

        bool hasErrors = discovery.Diagnostics.Any(static d => d.Severity == DiagnosticSeverity.Error);
        foreach (TContract contract in requiredContracts)
        {
            if (discovery.Contracts.ContainsKey(contract))
            {
                continue;
            }

            hasErrors = true;
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.MissingContractRegistration,
                Location.None,
                contract));
        }

        return hasErrors ? null : discovery.Contracts;
    }
}