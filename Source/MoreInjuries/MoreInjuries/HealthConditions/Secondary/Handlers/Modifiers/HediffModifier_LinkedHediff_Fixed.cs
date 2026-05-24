using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public partial class HediffModifier_LinkedHediff_Fixed : HediffModifier_LinkedHediff_Base
{
    [XmlBinding("chanceModifier", NullableBackingField = true)]
    public partial float ChanceModifier { get; }

    /// <inheritdoc />
    public override float GetModifier(Hediff hediff, IHediffCompHandler compHandler)
    {
        if (hediff.pawn.health.hediffSet.HasHediff(HediffDef))
        {
            return ChanceModifier;
        }
        return Unchanged;
    }
}
