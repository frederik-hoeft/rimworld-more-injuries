using Verse;

namespace MoreInjuries.AI.Audio;

public static class CachedSoundDefProvider
{
    public static ISoundDefProvider<TTarget> Of<TTarget>(SoundDef soundDef) where TTarget : ThingWithComps => 
        FixedSoundDefProviderCache<TTarget>.Get(soundDef);
}