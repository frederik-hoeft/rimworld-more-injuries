using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal sealed record ParsedBindingAttribute(
    string FieldName,
    DefaultValueSpec DefaultValue,
    bool AllowRawAccess,
    AttributeData Attribute);
