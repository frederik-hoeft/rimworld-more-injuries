using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;

namespace MoreInjuries.Roslyn.SourceGen.Settings;

[Generator(LanguageNames.CSharp)]
public sealed class SettingsGenerator : IIncrementalGenerator
{
    private const string SETTINGS_ENTRY_ATTRIBUTE_FULL_NAME = "MoreInjuries.Roslyn.Metadata.Settings.SettingsEntryAttribute<T>";
    private const string GENERATED_SETTINGS_ATTRIBUTE_FULL_NAME = "MoreInjuries.Roslyn.Metadata.Settings.GeneratedSettingsAttribute";
    private const string SCRIBE_VALUES_FULL_NAME = "Verse.Scribe_Values";
    private const string SCRIBE_VALUES_LOOK = "Look";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<Model> pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: GENERATED_SETTINGS_ATTRIBUTE_FULL_NAME,
            predicate: static (syntaxNode, cancellationToken) => syntaxNode is ClassDeclarationSyntax,
            transform: static (context, cancellationToken) =>
            {
                if (context.TargetSymbol is not INamedTypeSymbol targetClass)
                {
                    throw new InvalidOperationException($"{nameof(SettingsGenerator)}: Target symbol is not a class. Expected {nameof(INamedTypeSymbol)}, got {context.TargetSymbol.Kind}.");
                }
                AttributeData generatedSettingsAttribute = context.Attributes[0];
                string? customExposeDataMethodName = generatedSettingsAttribute.NamedArguments.FirstOrDefault(static arg => arg.Key == "ExposeData").Value.Value as string;
                IMethodSymbol? customExposeDataMethod = null;
                IMethodSymbol? exposeDataMethod = null;
                List<SettingsEntry> settingsEntries = [];
                foreach (ISymbol member in targetClass.GetMembers())
                {
                    if (member is IMethodSymbol method)
                    {
                        if (method is { DeclaredAccessibility: Accessibility.Public, IsOverride: true, ReturnType.SpecialType: SpecialType.System_Void, Name: "ExposeData", Parameters.Length: 0 })
                        {
                            exposeDataMethod = method;
                        }
                        else if (method.Name == customExposeDataMethodName && method.Parameters.Length == 0)
                        {
                            customExposeDataMethod = method;
                        }
                        continue;
                    }
                    if (member is not IPropertySymbol property)
                    {
                        continue;
                    }
                    AttributeData? attribute = null;
                    foreach (AttributeData attr in property.GetAttributes())
                    {
                        if (attr.AttributeClass?.ConstructedFrom.ToDisplayString() is string attributeFullName && attributeFullName == SETTINGS_ENTRY_ATTRIBUTE_FULL_NAME)
                        {
                            attribute = attr;
                            break;
                        }
                    }
                    if (attribute is null)
                    {
                        continue;
                    }
                    string name = property.Name;
                    TypedConstant? defaultValue = default;
                    foreach (KeyValuePair<string, TypedConstant> argument in attribute.NamedArguments)
                    {
                        if (argument.Key == "Name")
                        {
                            name = argument.Value.Value as string ?? name;
                        }
                        else if (argument.Key == "DefaultValue")
                        {
                            defaultValue = argument.Value;
                        }
                    }
                    settingsEntries.Add(new SettingsEntry(property, attribute, name, defaultValue));
                }
                return new Model
                (
                    Class: targetClass,
                    Namespace: targetClass.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)),
                    ClassName: targetClass.Name,
                    SettingsEntries: [.. settingsEntries],
                    CustomExposeDataMethodName: customExposeDataMethodName,
                    ExposeDataMethod: exposeDataMethod,
                    CustomExposeDataMethod: customExposeDataMethod
                );
            }
        );
        context.RegisterSourceOutput(pipeline, static (context, model) =>
        {
            if (model.ExposeDataMethod is not { IsPartialDefinition: true })
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        new DiagnosticDescriptor(
                            id: "MIRSG002",
                            title: "Invalid Settings Class",
                            messageFormat: "The class '{0}' must declare a 'public override partial void ExposeData()' method.",
                            category: "Settings",
                            defaultSeverity: DiagnosticSeverity.Error,
                            isEnabledByDefault: true
                        ),
                        model.Class.Locations.FirstOrDefault() ?? Location.None,
                        model.Class.Name
                    )
                );
                return;
            }
            if (!string.IsNullOrEmpty(model.CustomExposeDataMethodName) && model.CustomExposeDataMethod is null)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        new DiagnosticDescriptor(
                            id: "MIRSG003",
                            title: "Invalid Settings Class",
                            messageFormat: "Class '{0}' declares a custom expose data method '{1}', but no such method exists. Ensure a parameterless method with the same name exists in the class.",
                            category: "Settings",
                            defaultSeverity: DiagnosticSeverity.Error,
                            isEnabledByDefault: true
                        ),
                        model.Class.Locations.FirstOrDefault() ?? Location.None,
                        model.Class.Name,
                        model.CustomExposeDataMethodName
                    )
                );
                return;
            }
            Dictionary<string, string> backingFieldResolver = new(capacity: model.SettingsEntries.Length);
            Dictionary<string, string> defaultValueResolver = new(capacity: model.SettingsEntries.Length);
            const string DEFAULT_CLASS_NAME = "Defaults";
            StringBuilder bobTheBuilder = new();
            bobTheBuilder.AppendLine(
                $$"""
                #nullable enable

                namespace {{model.Namespace}};

                partial class {{model.ClassName}}
                {
                """);
            string indent = new(' ', 4);
            foreach (SettingsEntry entry in model.SettingsEntries)
            {
                if (entry.Property is not { RefKind: RefKind.Ref, IsPartialDefinition: true })
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            new DiagnosticDescriptor(
                                id: "MIRSG000",
                                title: "Invalid Settings Entry",
                                messageFormat: "The property '{0}' must be a partial ref property.",
                                category: "Settings",
                                defaultSeverity: DiagnosticSeverity.Error,
                                isEnabledByDefault: true
                            ),
                            entry.Property.Locations.FirstOrDefault() ?? Location.None,
                            entry.Property.Name
                        )
                    );
                    continue;
                }
                if (!SymbolEqualityComparer.Default.Equals(entry.Property.Type, entry.Attribute.AttributeClass?.TypeArguments[0]))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            new DiagnosticDescriptor(
                                id: "MIRSG001",
                                title: "Invalid Settings Entry",
                                messageFormat: "The type of the property '{0}' does not match the type argument of the attribute '{1}'.",
                                category: "Settings",
                                defaultSeverity: DiagnosticSeverity.Error,
                                isEnabledByDefault: true
                            ),
                            entry.Property.Locations.FirstOrDefault() ?? Location.None,
                            entry.Property.Name,
                            entry.Attribute.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                        )
                    );
                    continue;
                }
                string backingFieldName = SymbolNameGenerator.MakeUnique($"_{ToCamelCase(entry.Property.Name)}");
                string defaultValueName = $"{ToScreamingSnakeCase(entry.Property.Name)}_DEFAULT";
                backingFieldResolver[entry.Property.Name] = backingFieldName;
                defaultValueResolver[entry.Property.Name] = defaultValueName; 
                bobTheBuilder.AppendLine(
                    $$"""
                    {{indent}}[global::{{typeof(CompilerGeneratedAttribute).FullName}}]
                    {{indent}}private {{entry.Property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}} {{backingFieldName}} = {{DEFAULT_CLASS_NAME}}.{{defaultValueName}};
                    """);
            }
            foreach (SettingsEntry entry in model.SettingsEntries)
            {
                if (!backingFieldResolver.TryGetValue(entry.Property.Name, out string? backingFieldName))
                {
                    continue;
                }
                bobTheBuilder.AppendLine(
                    $$"""

                    {{indent}}{{SyntaxFacts.GetText(entry.Property.DeclaredAccessibility)}} partial ref {{entry.Property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}} {{entry.Property.Name}} => ref {{backingFieldName}};
                    """);
            }

            bobTheBuilder.AppendLine(
                $$"""

                {{indent}}public override partial void ExposeData()
                {{indent}}{
                """);

            foreach (SettingsEntry entry in model.SettingsEntries)
            {
                if (!defaultValueResolver.TryGetValue(entry.Property.Name, out string? defaultValueName))
                {
                    continue;
                }
                bobTheBuilder.AppendLine(
                    $$"""
                    {{indent}}    global::{{SCRIBE_VALUES_FULL_NAME}}.{{SCRIBE_VALUES_LOOK}}(ref {{entry.Property.Name}}, {{SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(entry.Name)).ToFullString()}}, {{DEFAULT_CLASS_NAME}}.{{defaultValueName}});
                    """);
            }
            bobTheBuilder.AppendLine(
                $$"""

                {{indent}}    base.ExposeData();
                """);
            if (model.CustomExposeDataMethod is not null)
            {
                bobTheBuilder.AppendLine(
                    $$"""

                    {{indent}}    {{model.CustomExposeDataMethod.Name}}();
                    """);
            }
            bobTheBuilder.AppendLine($$"""{{indent}}}""");

            bobTheBuilder.AppendLine(
                $$"""

                {{indent}}[global::{{typeof(CompilerGeneratedAttribute).FullName}}]
                {{indent}}public static class {{DEFAULT_CLASS_NAME}}
                {{indent}}{
                """);
            foreach (SettingsEntry entry in model.SettingsEntries)
            {
                if (!defaultValueResolver.TryGetValue(entry.Property.Name, out string? defaultValueName))
                {
                    continue;
                }
                if (entry.DefaultValue.HasValue)
                {
                    bobTheBuilder.AppendLine(
                        $$"""
                        {{indent}}    public const {{entry.Property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}} {{defaultValueName}} = {{entry.DefaultValue.Value.ToCSharpStringWithPostfix()}};
                        """);
                }
                else
                {
                    bobTheBuilder.AppendLine(
                        $$"""
                        {{indent}}    public const {{entry.Property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}} {{defaultValueName}} = default;
                        """);
                }
            }
            bobTheBuilder.AppendLine(
                $$"""
                {{indent}}}
                }
                """);

            SourceText sourceText = SourceText.From(bobTheBuilder.ToString(), Encoding.UTF8);

            context.AddSource($"{model.ClassName}.Settings.g.cs", sourceText);
        });
    }

    private static string ToScreamingSnakeCase(string input)
    {
        StringBuilder result = new();
        char previousChar = '\0';
        foreach (char c in input)
        {
            if (char.IsUpper(c) && (char.IsLower(previousChar) || char.IsDigit(previousChar))
                || char.IsDigit(c) && !char.IsDigit(previousChar))
            {
                result.Append('_');
            }
            result.Append(char.ToUpperInvariant(c));
            previousChar = c;
        }
        return result.ToString();
    }

    private static string ToCamelCase(string input) => char.ToLowerInvariant(input[0]) + input[1..];

    private record Model
    (
        INamedTypeSymbol Class,
        string Namespace,
        string ClassName,
        ImmutableArray<SettingsEntry> SettingsEntries,
        string? CustomExposeDataMethodName,
        IMethodSymbol? ExposeDataMethod,
        IMethodSymbol? CustomExposeDataMethod
    );

    private readonly record struct SettingsEntry(IPropertySymbol Property, AttributeData Attribute, string Name, TypedConstant? DefaultValue);
}
