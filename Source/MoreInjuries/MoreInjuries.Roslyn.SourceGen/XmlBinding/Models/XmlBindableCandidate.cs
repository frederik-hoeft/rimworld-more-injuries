using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

internal sealed record XmlBindableCandidate(XmlBindableGenerationModel? Model, ImmutableArray<Diagnostic> Diagnostics);
