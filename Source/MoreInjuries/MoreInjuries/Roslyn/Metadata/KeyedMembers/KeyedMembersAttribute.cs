namespace MoreInjuries.Roslyn.Metadata.KeyedMembers;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class KeyedMembersAttribute : Attribute
{
    public Visibility Visibility { get; set; } = Visibility.Public;
}