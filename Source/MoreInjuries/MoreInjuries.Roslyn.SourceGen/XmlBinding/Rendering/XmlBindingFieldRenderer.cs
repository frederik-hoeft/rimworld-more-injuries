using Microsoft.CodeAnalysis.CSharp;
using MoreInjuries.Roslyn.SourceGen.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;
using System.Runtime.CompilerServices;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Rendering;

internal static class XmlBindingFieldRenderer
{
    public static void BuildRegion(XmlBindableGenerationModel model, IndentedStringBuilder builder)
    {
        foreach (XmlBindingModel member in model.AnnotatedMembers)
        {
            BuildField(builder, member);
        }
    }

    private static void BuildField(IndentedStringBuilder builder, XmlBindingModel member)
    {
        string defaultExpression = member.DefaultValueExpression ?? "default";

        builder.AppendLine($"[global::{typeof(CompilerGeneratedAttribute).FullName}]");
        BuildRawAccessAttribute(builder, member);
        BuildDecorateAttribute(builder, member);
        BuildMayRequireAttribute(builder, member);
        builder.AppendLine($"private {GetReadonlyModifier(member)}{member.FieldTypeDisplay} {member.FieldName} = {defaultExpression};");
    }

    private static void BuildRawAccessAttribute(IndentedStringBuilder builder, XmlBindingModel member)
    {
        if (!member.AllowRawAccess)
        {
            builder.AppendLine($"[global::{typeof(ObsoleteAttribute).FullName}(\"Do not use this field directly. Use the corresponding property instead.\", error: false)]");
        }
    }

    private static void BuildDecorateAttribute(IndentedStringBuilder builder, XmlBindingModel member)
    {
        if (member.DecorateAttributeDisplay is { } decorateAttribute)
        {
            builder.AppendLine($"[{decorateAttribute}]");
        }
    }

    private static void BuildMayRequireAttribute(IndentedStringBuilder builder, XmlBindingModel member)
    {
        if (member.MayRequire is { } mayRequire)
        {
            string mayRequireLiteral = SymbolDisplay.FormatLiteral(mayRequire, quote: true);
            builder.AppendLine($"[global::RimWorld.MayRequireAttribute({mayRequireLiteral})]");
        }
    }

    private static string GetReadonlyModifier(XmlBindingModel member) =>
        member.Setter switch
        {
            { IsInitOnly: false } => string.Empty,
            _ => "readonly ",
        };
}
