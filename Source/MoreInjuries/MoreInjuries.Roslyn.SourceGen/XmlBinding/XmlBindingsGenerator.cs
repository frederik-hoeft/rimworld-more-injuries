using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Rendering;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding;

[Generator(LanguageNames.CSharp)]
public sealed class XmlBindingsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static postInit =>
        {
            postInit.AddEmbeddedSource<XmlBindableAttribute>();
            postInit.AddEmbeddedSource<XmlBindingAttribute>();
            postInit.AddEmbeddedSource(typeof(XmlBindingAttribute<>));
            postInit.AddEmbeddedSource(typeof(XmlFieldThrowHelper));
        });

        IncrementalValuesProvider<XmlBindableTarget> targets = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: typeof(XmlBindableAttribute).FullName,
            predicate: static (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax,
            transform: static (attributeContext, _) => XmlBindableTarget.Create(attributeContext));

        context.RegisterSourceOutput(targets, static (spc, target) =>
        {
            XmlBindableCandidate candidate = XmlBindableCandidateFactory.Create(target);

            foreach (Diagnostic diagnostic in candidate.Diagnostics)
            {
                spc.ReportDiagnostic(diagnostic);
            }

            if (candidate.Model is { } model)
            {
                string source = XmlBindingsRenderer.Render(model);
                spc.AddSource($"{model.ClassName}.XmlBindings.g.cs", source);
            }
        });
    }
}
