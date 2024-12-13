using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.HealthConditions.Choking;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = "Members initialized via XML defs")]
public class HediffCompProperties_Choking : HediffCompProperties
{
    // don't rename this field. XML defs depend on this name
    private readonly int _chokingIntervalTicks = default!;

    public HediffCompProperties_Choking() => compClass = typeof(HediffComp_Choking);

    public int ChokingIntervalTicks => _chokingIntervalTicks;
}