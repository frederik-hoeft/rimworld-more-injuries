using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.Extensions;

namespace MoreInjuries.Roslyn.SourceGen.Contracts;

[Generator(LanguageNames.CSharp)]
public sealed class ContractRegistrationBootstrapGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context) => context.RegisterPostInitializationOutput(EmitFrameworkSources);

    private static void EmitFrameworkSources(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddEmbeddedSource(typeof(GeneratorContractRegistrationAttribute<>));
    }
}
