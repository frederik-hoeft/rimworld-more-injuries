using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlSerializable]
public partial class HediffModifier_LinkedHediff_SimpleCurve : SecondaryHediffModifier
{
    [XmlMember("severityCurve")]
    public partial SimpleCurve SeverityCurve { get; }

    [XmlMember("hediffDef")]
    public partial HediffDef HediffDef { get; }

    /// <inheritdoc />
    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler)
    {
        if (hediff.pawn.health.hediffSet.TryGetHediff(HediffDef, out Hediff? linkedHediff))
        {
            // if the hediff exists, we evaluate the chance based on the severity curve
            return SeverityCurve.Evaluate(linkedHediff.Severity);
        }
        // if the hediff does not exist, we return the base chance
        return 1f;
    }
}
