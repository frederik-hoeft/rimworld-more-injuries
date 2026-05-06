using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.AI.TreatmentModifiers;

[XmlSerializable]
public abstract partial class TreatmentModifier_LinkedHediff : TreatmentModifier
{
    [XmlMember("otherHediffDef")]
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
