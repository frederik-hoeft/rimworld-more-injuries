using Verse;

namespace MoreInjuries.AI.Audio;

public sealed class FixedSoundDefProvider<TTarget>(SoundDef soundDef) : ISoundDefProvider<TTarget> where TTarget : ThingWithComps
{
    public SoundDef GetSoundDef(Pawn doctor, TTarget target, Thing? thing) => soundDef;
}
