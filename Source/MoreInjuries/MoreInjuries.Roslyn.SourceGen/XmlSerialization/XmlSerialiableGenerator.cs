using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MoreInjuries.Roslyn.SourceGen.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;

namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization;

[Generator(LanguageNames.CSharp)]
public sealed class XmlSerialiableGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static postInit =>
        {
            postInit.AddEmbeddedSource<XmlSerializableAttribute>();
            postInit.AddEmbeddedSource<XmlMemberAttribute>();
            postInit.AddEmbeddedSource(typeof(XmlMemberAttribute<>));
        });

        IncrementalValuesProvider<XmlSerialiableTarget> targets = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: typeof(XmlSerializableAttribute).FullName,
            predicate: static (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax,
            transform: static (attributeContext, _) => XmlSerialiableTarget.Create(attributeContext));

        context.RegisterSourceOutput(targets, static (spc, target) =>
        {
            XmlSerializableCandidate candidate = XmlSerializableCandidateFactory.Create(target);

            foreach (Diagnostic diagnostic in candidate.Diagnostics)
            {
                spc.ReportDiagnostic(diagnostic);
            }

            if (candidate.Model is { } model)
            {
                string source = XmlSerialiableRenderer.Render(model);
                spc.AddSource($"{model.ClassSymbol.Name}.XmlSerializable.g.cs", source);
            }
        });
    }
}