using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen;

internal static class Diagnostics
{
    private const string CATEGORY = "MoreInjuries.Roslyn.SourceGen";

    public static DiagnosticDescriptor MissingContractRegistration { get; } = new(
        id: "MI001",
        title: "Missing generator contract registration",
        messageFormat: "The required contract '{0}' is not registered for the module loader factory generator.",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor DuplicateContractRegistration { get; } = new(
        id: "MI002",
        title: "Duplicate generator contract registration",
        messageFormat: "The contract '{0}' is registered more than once. Existing registration: '{1}'. Duplicate: '{2}'.",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
