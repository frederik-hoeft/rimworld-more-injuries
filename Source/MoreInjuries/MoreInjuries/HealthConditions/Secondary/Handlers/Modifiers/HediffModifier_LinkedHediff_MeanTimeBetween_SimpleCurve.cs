using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlSerializable]
public sealed partial class HediffModifier_LinkedHediff_MeanTimeBetween_SimpleCurve : HediffModifier_MeanTimeBetween_SimpleCurve
{
    [XmlMember("hediffDef")]
    public partial HediffDef HediffDef { get; }

    public override float GetModifier(Hediff hediff, IHediffComp_TickHandler compHandler)
    {
        if (!hediff.pawn.health.hediffSet.TryGetHediff(HediffDef, out Hediff? linkedHediff))
        {
            return 1f;
        }
        float mttf = MttfDaysBySeverity.Evaluate(linkedHediff.Severity);
        return GetChanceFromMttf(mttf, compHandler.TickInterval);
    }
}
