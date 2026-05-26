using Microsoft.CodeAnalysis.CSharp;
using MoreInjuries.Roslyn.SourceGen.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Rendering;

internal static class XmlBindingPropertyRenderer
{
    private static readonly string s_throwHelperPrefix = $"global::{typeof(XmlFieldThrowHelper).FullName}";

    public static void BuildRegion(XmlBindableGenerationModel model, IndentedStringBuilder builder)
    {
        foreach (XmlBindingModel member in model.AnnotatedMembers)
        {
            BuildProperty(builder, member, model.ClassName);
        }
    }

    private static void BuildProperty(IndentedStringBuilder builder, XmlBindingModel member, string className)
    {
        switch (member)
        {
            case { HasSetter: false, GetterPipeline.Transform: null, GetterPipeline.Validate: null }:
                BuildExpressionBodiedProperty(builder, member, className);
                return;
            default:
                BuildBlockProperty(builder, member, className);
                return;
        }
    }

    private static void BuildExpressionBodiedProperty(IndentedStringBuilder builder, XmlBindingModel member, string className)
    {
        string getterExpression = GetGetterExpression(member, className);
        builder.AppendLine($"{GetPropertyDeclarationPrefix(member)} {member.PropertyName} => {getterExpression};");
    }

    private static void BuildBlockProperty(IndentedStringBuilder builder, XmlBindingModel member, string className)
    {
        builder.AppendLine($"{GetPropertyDeclarationPrefix(member)} {member.PropertyName}");
        builder.AppendLine("{");
        IndentedStringBuilder inner = builder.IncreaseIndent();

        BuildGetter(inner, member, className);
        BuildSetter(inner, member);

        builder.AppendLine("}");
    }

    private static void BuildGetter(IndentedStringBuilder builder, XmlBindingModel member, string className)
    {
        switch (member.GetterPipeline)
        {
            case { Transform: null, Validate: null }:
                BuildExpressionGetter(builder, member, className);
                return;
            default:
                BuildBlockGetter(builder, member, className);
                return;
        }
    }

    private static void BuildExpressionGetter(IndentedStringBuilder builder, XmlBindingModel member, string className)
    {
        string getterExpression = GetGetterExpression(member, className);
        builder.AppendLine($"get => {getterExpression};");
    }

    private static void BuildBlockGetter(IndentedStringBuilder builder, XmlBindingModel member, string className)
    {
        builder.AppendLine("get");
        builder.AppendLine("{");
        IndentedStringBuilder inner = builder.IncreaseIndent();

        string baseExpression = GetGetterExpression(member, className);
        inner.AppendLine($"{member.PropertyTypeDisplay} __value = {baseExpression};");
        BuildTransform(inner, member.GetterPipeline.Transform);
        BuildValidation(inner, member, className);
        inner.AppendLine("return __value;");

        builder.AppendLine("}");
    }

    private static void BuildTransform(IndentedStringBuilder builder, MethodCallModel? transform)
    {
        if (transform is { })
        {
            builder.AppendLine($"__value = {GetCallPrefix(transform)}{transform.MethodName}(__value);");
        }
    }

    private static void BuildValidation(IndentedStringBuilder builder, XmlBindingModel member, string className)
    {
        if (member.GetterPipeline.Validate is not { } validate)
        {
            return;
        }

        builder.AppendLine($"if (!{GetCallPrefix(validate)}{validate.MethodName}(__value))");
        builder.AppendLine("{");
        IndentedStringBuilder throwInner = builder.IncreaseIndent();
        throwInner.AppendLine($"{s_throwHelperPrefix}.{nameof(XmlFieldThrowHelper.FailedValidation)}(\"{className}\", \"{member.PropertyName}\");");
        builder.AppendLine("}");
    }

    private static void BuildSetter(IndentedStringBuilder builder, XmlBindingModel member)
    {
        if (member.Setter is { } setter)
        {
            builder.AppendLine($"{GetSetterAccessKeyword(setter, member)}{GetSetterKeyword(setter)} => this.{member.FieldName} = {GetSetterValueExpression(setter)};");
        }
    }

    private static string GetPropertyDeclarationPrefix(XmlBindingModel member) =>
        $"{SyntaxFacts.GetText(member.PropertyAccessibility)} {member.PropertyModifiers}partial {member.PropertyTypeDisplay}";

    private static string GetSetterAccessKeyword(SetterModel setter, XmlBindingModel member) =>
        setter.Accessibility switch
        {
            _ when setter.Accessibility != member.PropertyAccessibility => SyntaxFacts.GetText(setter.Accessibility) + " ",
            _ => string.Empty,
        };

    private static string GetSetterKeyword(SetterModel setter) =>
        setter.IsInitOnly switch
        {
            true => "init",
            false => "set",
        };

    private static string GetSetterValueExpression(SetterModel setter) =>
        setter.CastType switch
        {
            { } castType => $"({castType})value",
            null => "value",
        };

    private static string GetCallPrefix(MethodCallModel method) =>
        method.IsStatic switch
        {
            true => string.Empty,
            false => "this.",
        };

    private static string GetGetterExpression(XmlBindingModel member, string className) =>
        member.GetterPipeline switch
        {
            { RequiresNullCheck: false } => $"this.{member.FieldName}",
            { IsValueTypeNullCheck: true } => $"{s_throwHelperPrefix}.{nameof(XmlFieldThrowHelper.ValueNotNull)}(this.{member.FieldName}, \"{className}\")",
            { IsStringType: true } => $"{s_throwHelperPrefix}.{nameof(XmlFieldThrowHelper.NotNullOrEmpty)}(this.{member.FieldName}, \"{className}\")",
            _ => $"{s_throwHelperPrefix}.{nameof(XmlFieldThrowHelper.NotNull)}(this.{member.FieldName}, \"{className}\")",
        };
}
