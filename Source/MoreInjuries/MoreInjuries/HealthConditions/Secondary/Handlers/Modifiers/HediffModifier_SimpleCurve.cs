using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlSerializable]
public partial class HediffModifier_SimpleCurve : SecondaryHediffModifier
{
    [XmlMember("severityCurve")]
    public partial SimpleCurve SeverityCurve { get; }

    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler) =>
        SeverityCurve.Evaluate(hediff.Severity);
}
