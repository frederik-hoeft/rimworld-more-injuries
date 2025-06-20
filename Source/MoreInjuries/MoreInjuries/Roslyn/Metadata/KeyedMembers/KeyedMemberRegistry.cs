using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MoreInjuries.Roslyn.Metadata.KeyedMembers;

public sealed class KeyedMemberRegistry<TOwner>(Dictionary<string, KeyedMemberResolver<TOwner>> registry) where TOwner : class
{
    public bool TryGetMember<T>(TOwner instance, string feature, [MaybeNullWhen(false)] out T value)
    {
        if (registry.TryGetValue(feature, out KeyedMemberResolver<TOwner>? resolver))
        {
            object? result = resolver.Invoke(instance);
            if (result is T typedValue)
            {
                value = typedValue;
                return true;
            }
        }
        value = default;
        return false;
    }
}

public delegate object? KeyedMemberResolver<in T>(T instance) where T : class;