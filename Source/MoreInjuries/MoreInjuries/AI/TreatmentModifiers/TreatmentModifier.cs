using MoreInjuries.BuildIntrinsics;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.AI.TreatmentModifiers;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public abstract class TreatmentModifier
{
    // do not rename these fields. XML defs depend on these names
    private readonly JobDef jobDef = default!;

    public JobDef JobDef => jobDef;

    public abstract float GetEffectiveness(Hediff hediff);
}
