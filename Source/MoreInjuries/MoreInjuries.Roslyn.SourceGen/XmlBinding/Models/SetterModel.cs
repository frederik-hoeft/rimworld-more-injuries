using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

/// <summary>
/// Describes the setter configuration for a generated property. <see langword="null"/> means no setter.
/// </summary>
internal sealed record SetterModel(bool IsInitOnly, Accessibility Accessibility, string? CastType);
