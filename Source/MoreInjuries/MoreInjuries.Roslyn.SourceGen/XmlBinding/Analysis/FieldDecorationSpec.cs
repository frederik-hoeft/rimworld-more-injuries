namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal sealed record FieldDecorationSpec
(
    ParsedBindingAttribute ParsedAttribute,
    string? DecorateAttributeDisplay,
    string? MayRequire
);
