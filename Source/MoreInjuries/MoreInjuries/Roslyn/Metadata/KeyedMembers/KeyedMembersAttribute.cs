namespace MoreInjuries.Roslyn.Metadata.KeyedMembers;

/// <summary>
/// Marks a class for source generating bindings of contained fields.
/// A <see cref="KeyedMemberRegistry{TOwner}"/> will be generated for the class,
/// allowing reflection-free access to the fields by their names.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class KeyedMembersAttribute : Attribute
{
    public Visibility Visibility { get; set; } = Visibility.Public;
}