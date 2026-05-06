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
    /// When <see cref="DefaultValueProvider"/> is specified, the member is resolved on that type instead.
    /// </summary>
    public string? DefaultValueFrom { get; init; }

    /// <summary>
    /// Optional type that provides the static member referenced by <see cref="DefaultValueFrom"/>.
    /// When set, <see cref="DefaultValueFrom"/> must also be specified and refers to a static member
    /// on this type rather than on the declaring class.
    /// </summary>
    public Type? DefaultValueProvider { get; init; }

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
    /// (<c>Nullable&lt;T&gt;</c> for value types, <c>T?</c> for reference types) and is initialized to <c>null</c>.
    /// If the property type is non-nullable, the getter will throw when the backing field has not been initialized.
    /// This is mutually exclusive with <see cref="DefaultValueFrom"/> and <see cref="DefaultValueProvider"/>.
    /// </summary>
    public bool NullableBackingField { get; init; }

    /// <summary>
    /// When set to <see langword="true"/>, the generated backing field will not be marked with
    /// <see cref="ObsoleteAttribute"/>, allowing direct access without compiler warnings.
    /// </summary>
    public bool AllowRawAccess { get; init; }
}
