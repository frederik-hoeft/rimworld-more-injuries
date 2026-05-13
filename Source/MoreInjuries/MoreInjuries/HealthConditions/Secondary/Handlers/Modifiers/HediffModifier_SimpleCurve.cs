using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public partial class HediffModifier_SimpleCurve : SecondaryHediffModifier
{
    [XmlBinding("severityCurve")]
    public partial SimpleCurve SeverityCurve { get; }

    public override float GetModifier(Hediff hediff, IHediffCompHandler compHandler) =>
        SeverityCurve.Evaluate(hediff.Severity);
}
