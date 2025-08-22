using MoreInjuries.Defs;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public class HediffModifier_Reference : SecondaryHediffModifier
{
    // don't rename this field. XML defs depend on this name
    private readonly ReferenceableDef hediffModifierDef = default!;

    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler)
    {
        if (hediffModifierDef is null)
        {
            Logger.ConfigError($"{nameof(HediffModifier_Reference)} is not properly initialized. Current hediff modifier def is null. Cannot evaluate chance.");
            return 1f;
        }
        if (hediffModifierDef.GetModExtension<HediffModifierReference_ModExtension>() is not { Modifier: { } modifier })
        {
            Logger.ConfigError($"{nameof(HediffModifier_Reference)}: {hediffModifierDef.defName} does not have a valid HediffModifierReference_ModExtension. Cannot evaluate chance.");
            return 1f;
        }
        // if the hediff modifier exists, we return the modifier's chance
        return modifier.GetModifier(hediff, compHandler);
    }
}