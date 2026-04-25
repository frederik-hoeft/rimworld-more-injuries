namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class XmlMemberAttribute<T>(string name, T defaultValue) : Attribute
{
    public string Name { get; } = name;

    public T DefaultValue { get; } = defaultValue;
}
