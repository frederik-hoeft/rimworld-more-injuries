using MoreInjuries.AI.Audio;
using MoreInjuries.Defs.WellKnown;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs.Chloroform;

public sealed class ChloroformSoundDefProvider : ISoundDefProvider<Pawn>
{
    public SoundDef? GetSoundDef(Pawn doctor, Pawn target, Thing? thing) => target switch
    {
        { health.capacities.CanBeAwake: true, gender: Gender.Female } => KnownSoundDefOf.ChloroformFemale,
        { health.capacities.CanBeAwake: true } => KnownSoundDefOf.ChloroformMale,
        _ => null
    };
}
