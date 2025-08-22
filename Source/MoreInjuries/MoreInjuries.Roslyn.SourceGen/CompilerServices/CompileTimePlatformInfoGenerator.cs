using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Runtime.CompilerServices;
using System.Text;

namespace MoreInjuries.Roslyn.SourceGen.CompilerServices;

[Generator(LanguageNames.CSharp)]
public sealed class CompileTimePlatformInfoGenerator : IIncrementalGenerator
{
    private const string COMPILE_TIME_PLATFORM_INFO_ATTRIBUTE_FULL_NAME = "MoreInjuries.Roslyn.CompilerServices.CompileTimePlatformInfoAttribute";
    private const string COMPILE_TIME_PLATFORM_INFO_FULL_NAME = "MoreInjuries.Roslyn.CompilerServices.CompileTimePlatformInfo";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<Model> pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: COMPILE_TIME_PLATFORM_INFO_ATTRIBUTE_FULL_NAME,
            predicate: static (syntaxNode, cancellationToken) => syntaxNode is MethodDeclarationSyntax or PropertyDeclarationSyntax,
            transform: static (context, cancellationToken) => new Model
            (
                Target: context.TargetSymbol,
                Namespace: context.TargetSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)),
                Class: context.TargetSymbol.ContainingType
            ));
        context.RegisterSourceOutput(pipeline, static (context, model) =>
        {
            StringBuilder bobTheBuilder = new();
            string generatedFieldName = SymbolNameGenerator.MakeUnique("s_compileTimePlatformInfo");
            bobTheBuilder.AppendLine(
                $$"""
                #nullable enable

                namespace {{model.Namespace}};

                {{(model.Class.IsStatic ? "static " : string.Empty)}}partial class {{model.Class.Name}}
                {
                    [global::{{typeof(CompilerGeneratedAttribute).FullName}}]
                    private static readonly global::{{COMPILE_TIME_PLATFORM_INFO_FULL_NAME}} {{generatedFieldName}} = new
                    (
                        {{SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(Path.DirectorySeparatorChar)).ToFullString()}}
                    );

                    [global::{{typeof(CompilerGeneratedAttribute).FullName}}]
                """);
            bobTheBuilder.Append(' ', 4).Append(SyntaxFacts.GetText(model.Target.DeclaredAccessibility)).Append(' ');
            if (model.Target.IsStatic)
            {
                bobTheBuilder.Append(SyntaxFacts.GetText(SyntaxKind.StaticKeyword)).Append(' ');
            }
            bobTheBuilder.Append(SyntaxFacts.GetText(SyntaxKind.PartialKeyword)).Append(' ');
            bobTheBuilder.Append("global::").Append(COMPILE_TIME_PLATFORM_INFO_FULL_NAME).Append(' ').Append(model.Target.Name);
            if (model.Target is IMethodSymbol)
            {
                bobTheBuilder.Append("()");
            }
            bobTheBuilder.Append(" => ").Append(generatedFieldName).AppendLine(";").AppendLine("}");

            SourceText sourceText = SourceText.From(bobTheBuilder.ToString(), Encoding.UTF8);

            context.AddSource($"{model.Class.Name}.CompileTimePlatformInfo.g.cs", sourceText);
        });
    }

    private record Model(ISymbol Target, string Namespace, INamedTypeSymbol Class);
}
