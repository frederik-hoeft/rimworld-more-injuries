using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

internal sealed record XmlBindableCandidate(XmlBindableGenerationModel? Model, ImmutableArray<Diagnostic> Diagnostics);
