namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

/// <summary>
/// Describes the getter pipeline: null-check, transform, validate.
/// </summary>
internal sealed record GetterPipelineModel(
    bool RequiresNullCheck,
    bool IsStringType,
    bool IsValueTypeNullCheck,
    MethodCallModel? Transform,
    MethodCallModel? Validate);
