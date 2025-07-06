using MoreInjuries.BuildIntrinsics;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Linked;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public sealed class HediffCompProperties_LinkedSeverity : HediffCompProperties
{
    // don't rename this field. XML defs depend on this name
    private readonly int tickInterval = GenTicks.TickRareInterval;
    // don't rename this field. XML defs depend on this name
    private readonly float removeAtSeverity = 0f;

    public HediffCompProperties_LinkedSeverity() => compClass = typeof(HediffComp_LinkedSeverity);

    public int TickInterval => tickInterval;

    public float RemoveAtSeverity => removeAtSeverity;
}