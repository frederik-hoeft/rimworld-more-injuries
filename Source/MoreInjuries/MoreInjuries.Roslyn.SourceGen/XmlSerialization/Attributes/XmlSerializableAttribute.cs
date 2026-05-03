namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;

/// <summary>
/// Marks a class for XML serialization source generation. The generator will produce
/// backing fields and partial property implementations for all properties annotated
/// with <see cref="XmlMemberAttribute"/> or <see cref="XmlMemberAttribute{T}"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class XmlSerializableAttribute : Attribute;
