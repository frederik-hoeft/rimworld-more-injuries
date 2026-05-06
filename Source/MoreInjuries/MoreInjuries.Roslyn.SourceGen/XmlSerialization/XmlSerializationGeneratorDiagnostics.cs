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

    public static DiagnosticDescriptor DefaultValueFromMemberNotFound { get; } = new(
        id: "MIXML005",
        title: "defaultValueFrom member not found",
        messageFormat: $"Property '{{0}}' in class '{{1}}' uses an {nameof(XmlMemberAttribute)} with defaultValueFrom '{{2}}', but no primary constructor parameter or static member with that name was found.",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor DefaultValueFromTypeMismatch { get; } = new(
        id: "MIXML006",
        title: "defaultValueFrom type mismatch",
        messageFormat: $"Property '{{0}}' in class '{{1}}' uses an {nameof(XmlMemberAttribute)} with defaultValueFrom '{{2}}', but the source type '{{3}}' is not implicitly convertible to the backing field type '{{4}}'.",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor DefaultValueProviderMemberNotFound { get; } = new(
        id: "MIXML007",
        title: "DefaultValueProvider member not found",
        messageFormat: $"Property '{{0}}' in class '{{1}}' uses an {nameof(XmlMemberAttribute)} with DefaultValueProvider '{{2}}' and DefaultValueFrom '{{3}}', but no static member with that name was found on the provider type.",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor DefaultValueProviderRequiresDefaultValueFrom { get; } = new(
        id: "MIXML008",
        title: "DefaultValueProvider requires DefaultValueFrom",
        messageFormat: $"Property '{{0}}' in class '{{1}}' uses an {nameof(XmlMemberAttribute)} with DefaultValueProvider but DefaultValueFrom is not specified.",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor ValidateMethodNotFound { get; } = new(
        id: "MIXML009",
        title: "Validate method not found or invalid signature",
        messageFormat: $"Property '{{0}}' in class '{{1}}' uses an {nameof(XmlMemberAttribute)} with Validate '{{2}}', but no method with the signature '{{3}} -> bool' was found.",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor TransformMethodNotFound { get; } = new(
        id: "MIXML010",
        title: "Transform method not found or invalid signature",
        messageFormat: $"Property '{{0}}' in class '{{1}}' uses an {nameof(XmlMemberAttribute)} with Transform '{{2}}', but no method with the signature '{{3}} -> {{3}}' was found.",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
