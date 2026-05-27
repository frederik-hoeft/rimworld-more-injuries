using MoreInjuries.Defs;
using MoreInjuries.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlBindable]
public partial class HediffModifier_Reference : SecondaryHediffModifier
{
    [XmlBinding("hediffModifierDef")]
    public partial ReferenceableDef HediffModifierDef { get; }

    public override float GetModifier(Hediff hediff, IHediffCompHandler compHandler) =>
        // if the hediff modifier exists, we return the modifier's chance
        HediffModifierDef.GetRequiredModExtension<HediffModifierReference_ModExtension>().Modifier.GetModifier(hediff, compHandler);
}
