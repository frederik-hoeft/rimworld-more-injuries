using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.AI.TreatmentModifiers;

[XmlBindable]
public abstract partial class TreatmentModifier_LinkedHediff : TreatmentModifier
{
    [XmlBinding("otherHediffDef")]
    public partial HediffDef OtherHediffDef { get; }

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
