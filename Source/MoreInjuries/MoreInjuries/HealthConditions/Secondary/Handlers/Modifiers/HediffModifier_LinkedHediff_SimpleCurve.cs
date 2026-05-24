using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public partial class HediffModifier_LinkedHediff_SimpleCurve : HediffModifier_LinkedHediff_Base
{
    [XmlBinding("severityCurve")]
    public partial SimpleCurve SeverityCurve { get; }

    /// <inheritdoc />
    public override float GetModifier(Hediff hediff, IHediffCompHandler compHandler)
    {
        if (hediff.pawn.health.hediffSet.TryGetHediff(HediffDef, out Hediff? linkedHediff))
        {
            // if the hediff exists, we evaluate the chance based on the severity curve
            return SeverityCurve.Evaluate(linkedHediff.Severity);
        }
        return NoChange;
    }
}
