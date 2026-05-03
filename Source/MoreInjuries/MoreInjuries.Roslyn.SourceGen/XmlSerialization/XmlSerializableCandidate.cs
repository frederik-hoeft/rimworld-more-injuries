using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization;

internal sealed record XmlSerializableCandidate(XmlSerializableGenerationModel? Model, ImmutableArray<Diagnostic> Diagnostics);
