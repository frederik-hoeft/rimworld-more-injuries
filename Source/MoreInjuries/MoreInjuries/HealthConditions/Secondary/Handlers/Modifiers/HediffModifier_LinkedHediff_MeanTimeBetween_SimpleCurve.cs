using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public sealed partial class HediffModifier_LinkedHediff_MeanTimeBetween_SimpleCurve : HediffModifier_MeanTimeBetween_SimpleCurve
{
    [XmlBinding("hediffDef")]
    public partial HediffDef HediffDef { get; }

    public override float GetModifier(Hediff hediff, IHediffComp_TickHandler compHandler)
    {
        if (!hediff.pawn.health.hediffSet.TryGetHediff(HediffDef, out Hediff? linkedHediff))
        {
            return NoChange;
        }
        float mttf = MttfDaysBySeverity.Evaluate(linkedHediff.Severity);
        return GetChanceFromMttf(mttf, compHandler.TickInterval);
    }
}
