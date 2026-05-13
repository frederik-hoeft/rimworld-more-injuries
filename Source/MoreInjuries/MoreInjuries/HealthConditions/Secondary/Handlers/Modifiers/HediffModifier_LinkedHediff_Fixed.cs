using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public partial class HediffModifier_LinkedHediff_Fixed : SecondaryHediffModifier
{
    [XmlBinding("hediffDef")]
    public partial HediffDef HediffDef { get; }

    [XmlBinding("chanceModifier", NullableBackingField = true)]
    public partial float ChanceModifier { get; }

    /// <inheritdoc />
    public override float GetModifier(Hediff hediff, IHediffCompHandler compHandler)
    {
        if (hediff.pawn.health.hediffSet.HasHediff(HediffDef))
        {
            // if the hediff exists, we evaluate the chance based on the severity curve
            return ChanceModifier;
        }
        // if the hediff does not exist, we return the base chance
        return 1f;
    }
}
