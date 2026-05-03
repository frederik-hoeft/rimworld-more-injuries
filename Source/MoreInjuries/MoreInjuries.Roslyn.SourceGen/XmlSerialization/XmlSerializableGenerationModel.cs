using System.Collections.Immutable;

namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization;

internal sealed record XmlSerializableGenerationModel
(
    string Namespace,
    string ClassName,
    ImmutableArray<XmlMemberModel> AnnotatedMembers
);
