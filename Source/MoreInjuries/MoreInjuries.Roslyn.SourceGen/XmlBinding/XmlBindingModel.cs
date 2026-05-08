using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding;

/// <summary>
/// Describes the setter configuration for a generated property. <see langword="null"/> means no setter.
/// </summary>
internal sealed record SetterModel(bool IsInitOnly, Accessibility Accessibility, string? CastType);

/// <summary>
/// Describes a method call expression used in the getter pipeline (Transform or Validate).
/// </summary>
internal sealed record MethodCallModel(string MethodName, bool IsStatic);

/// <summary>
/// Describes the getter pipeline: null-check, transform, validate.
/// </summary>
internal sealed record GetterPipelineModel(
    bool RequiresNullCheck,
    bool IsStringType,
    bool IsValueTypeNullCheck,
    MethodCallModel? Transform,
    MethodCallModel? Validate);

/// <summary>
/// Immutable model for a single annotated XML member. Composed of focused sub-models.
/// </summary>
internal sealed record XmlBindingModel(
    string PropertyName,
    string PropertyTypeDisplay,
    string FieldTypeDisplay,
    Accessibility PropertyAccessibility,
    string PropertyModifiers,
    string FieldName,
    string? DefaultValueExpression,
    bool AllowRawAccess,
    SetterModel? Setter,
    GetterPipelineModel GetterPipeline
)
{
    public bool HasSetter => Setter is not null;
}
