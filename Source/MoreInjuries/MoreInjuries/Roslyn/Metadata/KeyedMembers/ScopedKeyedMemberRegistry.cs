namespace MoreInjuries.Roslyn.Metadata.KeyedMembers;

public readonly struct ScopedKeyedMemberRegistry<TOwner>(TOwner instance, KeyedMemberRegistry<TOwner> registry) where TOwner : class
{
    public bool TryGetMember<T>(string feature, [MaybeNullWhen(false)] out T value) =>
        registry.TryGetMember(instance, feature, out value);
}