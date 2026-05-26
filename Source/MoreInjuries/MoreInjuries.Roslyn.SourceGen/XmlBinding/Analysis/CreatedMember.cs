using MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

internal sealed record CreatedMember(string FieldName, XmlBindingModel Model);
