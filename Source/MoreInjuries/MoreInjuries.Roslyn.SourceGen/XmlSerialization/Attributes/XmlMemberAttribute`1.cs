namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;

/// <summary>
/// Annotates a partial property to generate a backing field with the specified XML-bound name
/// and a compile-time constant default value.
/// </summary>
/// <typeparam name="T">The type of the default value. Must match the property type.</typeparam>
/// <param name="name">
/// The name of the generated backing field. This must match the XML element name
/// used in RimWorld def files, as the game's XML deserializer populates fields by name via reflection.
/// </param>
/// <param name="defaultValue">
/// A compile-time constant used to initialize the backing field. When the XML def does not
/// provide a value, the field retains this default.
/// </param>
[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class XmlMemberAttribute<T>(string name, T defaultValue) : Attribute
{
    /// <summary>Gets the XML-bound backing field name.</summary>
    public string Name { get; } = name;

    /// <summary>Gets the compile-time constant default value for the backing field.</summary>
    public T DefaultValue { get; set; } = defaultValue;

    /// <summary>
    /// Optional name of a local method (instance or static) with the signature <c>propertyType -&gt; bool</c>.
    /// The method is invoked by the generated getter before returning, and an exception is thrown if it returns <see langword="false"/>.
    /// </summary>
    public string? Validate { get; init; }

    /// <summary>
    /// Optional name of a local method (instance or static) with the signature <c>propertyType -&gt; propertyType</c>.
    /// The method is invoked by the generated getter to transform the value before returning it.
    /// </summary>
    public string? Transform { get; init; }

    /// <summary>
    /// When set to <see langword="true"/>, the generated backing field uses the nullable form of the type
    /// (<c>Nullable&lt;T&gt;</c> for value types) and is initialized to <c>null</c>.
    /// If the property type is non-nullable, the getter will throw when the backing field has not been initialized.
    /// </summary>
    public bool NullableBackingField { get; init; }

    /// <summary>
    /// When set to <see langword="true"/>, the generated backing field will not be marked with
    /// <see cref="ObsoleteAttribute"/>, allowing direct access without compiler warnings.
    /// </summary>
    public bool AllowRawAccess { get; init; }
}
