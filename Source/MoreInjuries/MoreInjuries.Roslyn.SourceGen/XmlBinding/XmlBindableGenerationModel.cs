using System.Collections.Immutable;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding;

internal sealed record XmlBindableGenerationModel
(
    string Namespace,
    string ClassName,
    ImmutableArray<XmlBindingModel> AnnotatedMembers
);
