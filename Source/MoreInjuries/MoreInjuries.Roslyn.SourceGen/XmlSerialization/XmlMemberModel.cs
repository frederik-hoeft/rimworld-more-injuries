using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization;

internal sealed record XmlMemberModel
(
    string PropertyName,
    string PropertyTypeDisplay,
    string FieldTypeDisplay,
    Accessibility PropertyAccessibility,
    string FieldName,
    string? DefaultValueExpression,
    bool HasSetter,
    bool IsInitOnly,
    Accessibility SetterAccessibility,
    bool RequiresNullCheck,
    bool IsStringType
);
