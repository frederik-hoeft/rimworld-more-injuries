using System.Collections.Concurrent;
using Verse;

namespace MoreInjuries.AI.Audio;

internal static class FixedSoundDefProviderCache<TTarget> where TTarget : ThingWithComps
{
    private static readonly ConcurrentDictionary<SoundDef, ISoundDefProvider<TTarget>> s_cache = [];

    public static ISoundDefProvider<TTarget> Get(SoundDef soundDef)
    {
        if (s_cache.TryGetValue(soundDef, out ISoundDefProvider<TTarget>? provider))
        {
            return provider;
        }
        provider = new FixedSoundDefProvider<TTarget>(soundDef);
        if (s_cache.TryAdd(soundDef, provider))
        {
            return provider;
        }
        // If another thread added the provider, we return the existing one
        return s_cache[soundDef];
    }
}
