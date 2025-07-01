using MoreInjuries.BuildIntrinsics;
using Verse;

namespace MoreInjuries.AI.TreatmentModifiers;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public abstract class TreatmentModifier_LinkedHediff : TreatmentModifier
{
    // do not rename these fields. XML defs depend on these names
    private readonly HediffDef otherHediffDef = default!;

    public HediffDef OtherHediffDef => otherHediffDef;

    protected abstract float GetEffectiveness(Hediff hediff, Hediff otherHediff);

    public override float GetEffectiveness(Hediff hediff)
    {
        if (hediff.pawn.health.hediffSet.TryGetHediff(OtherHediffDef, out Hediff? otherHediff))
        {
            return GetEffectiveness(hediff, otherHediff);
        }
        return 1f;
    }
}
