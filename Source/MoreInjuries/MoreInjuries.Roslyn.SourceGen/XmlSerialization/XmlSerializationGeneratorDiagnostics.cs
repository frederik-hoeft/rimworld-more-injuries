using Microsoft.CodeAnalysis;
using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;

namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization;

internal static class XmlSerializationGeneratorDiagnostics
{
    private const string CATEGORY = "MoreInjuries.Roslyn.SourceGen.XmlSerialization";

    public static DiagnosticDescriptor TargetMustBeClass { get; } = new(
        id: "MIXML001",
        title: "Generator target must be a class",
        messageFormat: $"Type '{{0}}' must be a class to use {nameof(XmlSerializableAttribute)}.",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor MemberMustBePartial { get; } = new(
        id: "MIXML002",
        title: "Generator target must be partial",
        messageFormat: "Property '{0}' in class '{1}' must be partial to be used with the XML serialization source generator.",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidFieldName { get; } = new(
        id: "MIXML003",
        title: "Invalid field name",
        messageFormat: $"Property '{{0}}' in class '{{1}}' uses an {nameof(XmlMemberAttribute)} with an invalid member name '{{2}}'. The member name must be a valid C# identifier and must not conflict with any other member names in the same class.",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor MemberNameConflict { get; } = new(
        id: "MIXML004",
        title: "Member name conflict",
        messageFormat: $"Property '{{0}}' in class '{{1}}' uses an {nameof(XmlMemberAttribute)} with a member name '{{2}}' that conflicts with another member name in the same class. All member names must be unique.",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}