using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization;

internal sealed record XmlSerializableGenerationModel
(
    string Namespace,
    INamedTypeSymbol ClassSymbol,
    ImmutableArray<XmlMemberModel> AnnotatedMembers
);
