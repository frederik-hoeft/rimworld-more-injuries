using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlSerializable]
public partial class HediffModifier_MeanTimeBetween_SimpleCurve_Hours : HediffModifier_MeanTimeBetween
{
    [XmlMember("mttfHoursBySeverity")]
    public partial SimpleCurve MttfHoursBySeverity { get; }

    public override float GetModifier(Hediff hediff, IHediffComp_TickHandler compHandler)
    {
        float mttfHours = MttfHoursBySeverity.Evaluate(hediff.Severity);
        return GetChanceFromMttf(mttfHours * GenDate.TicksPerHour, compHandler.TickInterval);
    }
}
