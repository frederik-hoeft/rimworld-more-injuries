using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlSerializable]
public partial class HediffModifier_LinkedHediff_Fixed : SecondaryHediffModifier
{
    [XmlMember("hediffDef")]
    public partial HediffDef HediffDef { get; }

    [XmlMember("chanceModifier", NullableBackingField = true)]
    public partial float ChanceModifier { get; }

    /// <inheritdoc />
    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler)
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
