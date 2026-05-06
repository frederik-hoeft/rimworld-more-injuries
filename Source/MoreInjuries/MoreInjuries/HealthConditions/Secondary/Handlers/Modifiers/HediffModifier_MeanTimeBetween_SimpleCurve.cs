using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlSerializable]
public partial class HediffModifier_MeanTimeBetween_SimpleCurve : HediffModifier_MeanTimeBetween
{
    [XmlMember("mttfDaysBySeverity")]
    public partial SimpleCurve MttfDaysBySeverity { get; }

    public override float GetModifier(Hediff hediff, IHediffComp_TickHandler compHandler)
    {
        float mttfDays = MttfDaysBySeverity.Evaluate(hediff.Severity);
        return GetChanceFromMttf(mttfDays * GenDate.TicksPerDay, compHandler.TickInterval);
    }
}
