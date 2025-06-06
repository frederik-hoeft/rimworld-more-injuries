using MoreInjuries.BuildIntrinsics;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;

[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class WeightedHediffMakerDef(HediffDef hediffDef, float? minSeverity, float? maxSeverity, bool? allowDuplicate, bool? allowMultiple, float weight) 
    : HediffMakerDef(hediffDef, minSeverity, maxSeverity, allowDuplicate, allowMultiple)
{
    // don't rename this field. XML defs depend on this name
    private readonly float weight = weight;

    public WeightedHediffMakerDef() : this(default!, null, null, null, null, 1f) { }

    public float Weight => weight;
}
