using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

internal sealed record XmlBindingPropertyModel(
    string Name,
    string TypeDisplay,
    Accessibility Accessibility,
    string Modifiers,
    SetterModel? Setter);
