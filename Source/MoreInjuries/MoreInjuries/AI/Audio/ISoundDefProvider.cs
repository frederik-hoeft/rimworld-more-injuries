using Verse;

namespace MoreInjuries.AI.Audio;

public interface ISoundDefProvider<in TTarget> where TTarget : ThingWithComps
{
    /// <summary>
    /// Gets the sound definition for the given doctor and target.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="null"/> if no sound should be played.
    /// </remarks>
    SoundDef? GetSoundDef(Pawn doctor, TTarget target, Thing? thing);
}