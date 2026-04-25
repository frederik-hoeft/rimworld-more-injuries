namespace MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class XmlMemberAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}