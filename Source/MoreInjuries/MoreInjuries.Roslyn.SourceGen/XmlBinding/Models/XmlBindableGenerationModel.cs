using System.Collections.Immutable;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

internal sealed record XmlBindableGenerationModel
(
    string Namespace,
    string ClassName,
    ImmutableArray<XmlBindingModel> AnnotatedMembers
);
