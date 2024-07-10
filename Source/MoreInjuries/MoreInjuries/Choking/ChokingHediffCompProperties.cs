using Verse;

namespace MoreInjuries.Choking;

// members initialized via XML defs
public class ChokingHediffCompProperties : HediffCompProperties
{
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
    // don't rename this field. XML defs depend on this name
    private readonly int _chokingIntervalTicks = default!;

    // don't rename this field. XML defs depend on this name
    private readonly SoundDef _coughSoundDef = default!;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

    public ChokingHediffCompProperties() => compClass = typeof(ChokingHediffComp);

    public int ChokingIntervalTicks => _chokingIntervalTicks;

    public SoundDef CoughSoundDef => _coughSoundDef;
}