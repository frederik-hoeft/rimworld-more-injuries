using MoreInjuries.BuildIntrinsics;
using Verse;

namespace MoreInjuries.HealthConditions.Choking;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = "Members initialized via XML defs")]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class HediffCompProperties_Choking : HediffCompProperties
{
    // don't rename this field. XML defs depend on this name
    private readonly int chokingIntervalTicks = default!;

    public HediffCompProperties_Choking() => compClass = typeof(HediffComp_Choking);

    public int ChokingIntervalTicks => chokingIntervalTicks;
}