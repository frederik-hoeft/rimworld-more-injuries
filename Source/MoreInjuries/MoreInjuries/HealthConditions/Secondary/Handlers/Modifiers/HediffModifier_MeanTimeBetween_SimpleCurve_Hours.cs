using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public partial class HediffModifier_MeanTimeBetween_SimpleCurve_Hours : HediffModifier_MeanTimeBetween
{
    [XmlBinding("mttfHoursBySeverity")]
    public partial SimpleCurve MttfHoursBySeverity { get; }

    public override float GetModifier(Hediff hediff, IHediffComp_TickHandler compHandler)
    {
        float mttfHours = MttfHoursBySeverity.Evaluate(hediff.Severity);
        return GetChanceFromMttf(mttfHours * GenDate.TicksPerHour, compHandler.TickInterval);
    }
}
