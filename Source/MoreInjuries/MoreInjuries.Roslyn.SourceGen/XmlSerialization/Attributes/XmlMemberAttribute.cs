namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;

/// <summary>
/// Annotates a partial property to generate a backing field with the specified XML-bound name.
/// The source generator will emit the field and the property implementation.
/// </summary>
/// <param name="name">
/// The name of the generated backing field. This must match the XML element name
/// used in RimWorld def files, as the game's XML deserializer populates fields by name via reflection.
/// </param>
[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class XmlMemberAttribute(string name) : Attribute
{
    /// <summary>Gets the XML-bound backing field name.</summary>
    public string Name { get; } = name;

    /// <summary>
    /// Optional name of a primary constructor parameter or static member (field/property) whose value
    /// is used to initialize the backing field. The source type must be implicitly convertible to the property type.
    /// </summary>
    public string? DefaultValueFrom { get; init; }

    /// <summary>
    /// When set to <see langword="true"/>, the generated backing field will not be marked with
    /// <see cref="ObsoleteAttribute"/>, allowing direct access without compiler warnings.
    /// </summary>
    public bool AllowRawAccess { get; init; }
}
