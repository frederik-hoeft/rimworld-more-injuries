using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MoreInjuries.Roslyn.SourceGen.Extensions;
using System.Text;

namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization;

internal static class XmlSerialiableRenderer
{
    private static readonly SymbolDisplayFormat s_fullyQualifiedFormat =
        SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Included);

    public static string Render(XmlSerializableGenerationModel model)
    {
        StringBuilder sourceBuilder = new(
            $$"""
            #nullable enable

            {{RenderNamespace(model.Namespace)}}
            partial class {{model.ClassSymbol.Name}}
            {

            """);

        IndentedStringBuilder indentedBuilder = new(sourceBuilder, indentLevel: 1);
        BuildFieldRegion(model, indentedBuilder);
        indentedBuilder.Raw.AppendLine();
        BuildPropertyRegion(model, indentedBuilder);
        sourceBuilder.AppendLine("}");

        return sourceBuilder.ToString();
    }

    private static void BuildFieldRegion(XmlSerializableGenerationModel model, IndentedStringBuilder builder)
    {
        foreach (XmlMemberModel member in model.AnnotatedMembers)
        {
            builder.AppendLine($"private readonly {member.Property.Type.ToDisplayString(s_fullyQualifiedFormat)} {member.FieldName};");
        }
    }

    private static void BuildPropertyRegion(XmlSerializableGenerationModel model, IndentedStringBuilder builder)
    {
        foreach (XmlMemberModel member in model.AnnotatedMembers)
        {
            builder.Append(SyntaxFacts.GetText(member.Property.DeclaredAccessibility)).Append(" partial ").Append(member.Property.Type.ToDisplayString(s_fullyQualifiedFormat)).Append(" ").Append(member.Property.Name).Append(" => ").Append(member.FieldName).AppendLine(";");
        }
    }

    private static string RenderNamespace(string namespaceName) =>
        string.IsNullOrWhiteSpace(namespaceName)
            ? string.Empty
            : $"namespace {namespaceName};\n";
}