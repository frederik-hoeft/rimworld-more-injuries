using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.HealthConditions.Choking;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = "Members initialized via XML defs")]
public class ChokingHediffCompProperties : HediffCompProperties
{
    // don't rename this field. XML defs depend on this name
    private readonly int _chokingIntervalTicks = default!;

    // don't rename this field. XML defs depend on this name
    private readonly SoundDef _coughSoundDef = default!;

    public ChokingHediffCompProperties() => compClass = typeof(ChokingHediffComp);

    public int ChokingIntervalTicks => _chokingIntervalTicks;

    public SoundDef CoughSoundDef => _coughSoundDef;
}