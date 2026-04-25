using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization;

internal sealed record XmlMemberModel(IPropertySymbol Property, string FieldName, string? DefaultValueExpression);