using MoreInjuries.Defs;
using MoreInjuries.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlSerializable]
public partial class HediffModifier_Reference : SecondaryHediffModifier
{
    [XmlMember("hediffModifierDef")]
    public partial ReferenceableDef HediffModifierDef { get; }

    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler) =>
        // if the hediff modifier exists, we return the modifier's chance
        HediffModifierDef.GetRequiredModExtension<HediffModifierReference_ModExtension>().Modifier.GetModifier(hediff, compHandler);
}
