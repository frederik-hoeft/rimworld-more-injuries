namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;

/// <summary>
/// Marks a class for XML binding source generation. The generator will produce
/// backing fields and partial property implementations for all properties annotated
/// with <see cref="XmlBindingAttribute"/> or <see cref="XmlBindingAttribute{T}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class XmlBindableAttribute : Attribute;
