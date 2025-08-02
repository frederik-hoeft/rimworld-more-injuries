using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Runtime.CompilerServices;
using System.Text;

namespace MoreInjuries.Roslyn.SourceGen.KeyedMembers;

[Generator(LanguageNames.CSharp)]
public sealed class KeyedMemberGenerator : IIncrementalGenerator
{
    private const string KEYED_MEMBERS_ATTRIBUTE_FULL_NAME = "MoreInjuries.Roslyn.Metadata.KeyedMembers.KeyedMembersAttribute";
    private const string KEYED_MEMBERS_ATTRIBUTE_VISIBILITY_NAME = "Visibility";
    private const string KEYED_MEMBER_REGISTRY_FULL_NAME = "MoreInjuries.Roslyn.Metadata.KeyedMembers.KeyedMemberRegistry";
    private const string FEATURE_FLAG_RESOLVER_FULL_NAME = "MoreInjuries.Roslyn.Metadata.KeyedMembers.KeyedMemberResolver";
    private const string SCOPED_KEYED_MEMBER_REGISTRY_FULL_NAME = "MoreInjuries.Roslyn.Metadata.KeyedMembers.ScopedKeyedMemberRegistry";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<Model> pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: KEYED_MEMBERS_ATTRIBUTE_FULL_NAME,
            predicate: static (syntaxNode, cancellationToken) => syntaxNode is ClassDeclarationSyntax,
            transform: static (context, cancellationToken) =>
            {
                if (context.TargetSymbol is not INamedTypeSymbol targetClass)
                {
                    throw new InvalidOperationException($"{nameof(KeyedMemberGenerator)}: Target symbol is not a class. Expected INamedTypeSymbol, got {context.TargetSymbol.Kind}.");
                }
                AttributeData keyedMembersAttribute = context.Attributes[0];
                return new Model
                (
                    Class: targetClass,
                    Namespace: targetClass.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)),
                    ClassName: targetClass.Name,
                    Attribute: keyedMembersAttribute,
                    Visibility: keyedMembersAttribute.NamedArguments.FirstOrDefault(static arg => arg.Key is KEYED_MEMBERS_ATTRIBUTE_VISIBILITY_NAME).Value.Value switch
                    {
                        0 => "public",
                        1 => "internal",
                        2 => "protected",
                        3 => "private",
                        4 => "protected internal",
                        5 => "private protected",
                        _ => "public" // default to public if not specified or invalid value
                    }
                );
            }
        );
        context.RegisterSourceOutput(pipeline, static (context, model) =>
        {
            StringBuilder bobTheBuilder = new();
            string fieldName = SymbolNameGenerator.MakeUnique("s_keyedMemberRegistry");
            bobTheBuilder.AppendLine(
                $$"""
                #nullable enable

                namespace {{model.Namespace}};

                partial class {{model.ClassName}}
                {
                    [global::{{typeof(CompilerGeneratedAttribute).FullName}}]
                    private static readonly global::{{KEYED_MEMBER_REGISTRY_FULL_NAME}}<{{model.ClassName}}> {{fieldName}} = new(global::{{typeof(Enumerable).FullName}}.{{nameof(Enumerable.ToDictionary)}}(((string Field, global::{{FEATURE_FLAG_RESOLVER_FULL_NAME}}<{{model.ClassName}}> Resolver)[])
                    [
                """);
            string indent = new(' ', 8);
            IEnumerable<ISymbol> fields = model.Class.GetMembers()
                .Where(static member => member is IFieldSymbol or IPropertySymbol 
                    && !member.IsStatic 
                    && member.GetAttributes().All(static attr => attr.AttributeClass?.Name != nameof(CompilerGeneratedAttribute)));
            foreach (ISymbol field in fields)
            {
                bobTheBuilder.AppendLine(
                    $"""
                    {indent}("{field.Name}", static instance => instance.{field.Name}),
                    """);
            }
            bobTheBuilder.AppendLine(
                $$"""
                    ], static kvp => kvp.Field, static kvp => kvp.Resolver));
                
                    [global::{{typeof(CompilerGeneratedAttribute).FullName}}]
                    {{model.Visibility}} global::{{SCOPED_KEYED_MEMBER_REGISTRY_FULL_NAME}}<{{model.ClassName}}> Keyed => new(this, {{fieldName}});
                }
                """);
            SourceText sourceText = SourceText.From(bobTheBuilder.ToString(), Encoding.UTF8);

            context.AddSource($"{model.ClassName}.Keyed.g.cs", sourceText);
        });
    }

    private record Model(INamedTypeSymbol Class, string Namespace, string ClassName, AttributeData Attribute, string Visibility);
}
