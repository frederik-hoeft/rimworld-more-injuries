using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public partial class HediffModifier_MeanTimeBetween_SimpleCurve : HediffModifier_MeanTimeBetween
{
    [XmlBinding("mttfDaysBySeverity")]
    public partial SimpleCurve MttfDaysBySeverity { get; }

    public override float GetModifier(Hediff hediff, IHediffComp_TickHandler compHandler)
    {
        float mttfDays = MttfDaysBySeverity.Evaluate(hediff.Severity);
        return GetChanceFromMttf(mttfDays * GenDate.TicksPerDay, compHandler.TickInterval);
    }
}
