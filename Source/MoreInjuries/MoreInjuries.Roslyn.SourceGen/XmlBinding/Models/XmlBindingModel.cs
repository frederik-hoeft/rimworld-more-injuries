using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Models;

/// <summary>
/// Immutable model for a single annotated XML member.
/// </summary>
internal sealed record XmlBindingModel(
    XmlBindingPropertyModel Property,
    XmlBindingFieldModel Field,
    GetterPipelineModel GetterPipeline)
{
    public string PropertyName => Property.Name;

    public string PropertyTypeDisplay => Property.TypeDisplay;

    public Accessibility PropertyAccessibility => Property.Accessibility;

    public string PropertyModifiers => Property.Modifiers;

    public SetterModel? Setter => Property.Setter;

    public bool HasSetter => Setter is not null;

    public string FieldName => Field.Name;

    public string FieldTypeDisplay => Field.TypeDisplay;

    public string? DefaultValueExpression => Field.DefaultValueExpression;

    public bool AllowRawAccess => Field.AllowRawAccess;

    public string? DecorateAttributeDisplay => Field.DecorateAttributeDisplay;

    public string? MayRequire => Field.MayRequire;
}
