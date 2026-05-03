using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.Extensions;

namespace MoreInjuries.Roslyn.SourceGen.Contracts;

internal sealed class ContractExplorer(Compilation compilation)
{
    public Compilation Compilation => compilation;

    public ContractDiscoveryResult<TContract> DiscoverContracts<TContract>() where TContract : unmanaged, Enum
    {
        List<Diagnostic> diagnostics = [];
        Dictionary<TContract, INamedTypeSymbol> discoveredContracts = [];
        IEnumerable<IAssemblySymbol> assemblies = compilation.References
            .Select(compilation.GetAssemblyOrModuleSymbol)
            .OfType<IAssemblySymbol>()
            .Append(compilation.Assembly)
            .Distinct<IAssemblySymbol>(SymbolEqualityComparer.Default);

        foreach (INamedTypeSymbol type in assemblies.SelectMany(static asm => asm.GlobalNamespace.GetAllTypes()))
        {
            foreach (AttributeData attribute in type.GetAttributes())
            {
                if (attribute.AttributeClass is not INamedTypeSymbol attributeClass || !attributeClass.IsGenericType)
                {
                    continue;
                }

                if (!attributeClass.ConstructUnboundGenericType().GetFullMetadataName().Equals(typeof(GeneratorContractRegistrationAttribute<>).FullName, StringComparison.Ordinal))
                {
                    continue;
                }

                if (attribute.AttributeClass.TypeArguments is not [INamedTypeSymbol registeredContractEnum]
                    || !registeredContractEnum.GetFullMetadataName().Equals(typeof(TContract).FullName, StringComparison.Ordinal))
                {
                    continue;
                }

                if (attribute.ConstructorArguments is not [TypedConstant enumValueConstant] || enumValueConstant.Value is null)
                {
                    continue;
                }

                TContract contractValue = (TContract)enumValueConstant.Value;
                if (!discoveredContracts.TryAdd(contractValue, type))
                {
                    diagnostics.Add(Diagnostic.Create(
                        Diagnostics.DuplicateContractRegistration,
                        type.Locations.FirstOrDefault(),
                        contractValue,
                        discoveredContracts[contractValue].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                        type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));
                }
            }
        }

        return new ContractDiscoveryResult<TContract>(discoveredContracts, [.. diagnostics]);
    }
}
