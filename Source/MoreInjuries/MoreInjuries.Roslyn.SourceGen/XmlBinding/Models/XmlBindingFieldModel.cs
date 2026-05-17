namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

internal sealed record XmlBindingFieldModel(
    string Name,
    string TypeDisplay,
    string? DefaultValueExpression,
    bool AllowRawAccess,
    string? DecorateAttributeDisplay,
    string? MayRequire);
