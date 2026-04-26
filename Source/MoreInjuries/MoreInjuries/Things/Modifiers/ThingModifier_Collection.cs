using System.Collections.Generic;
using Verse;

namespace MoreInjuries.Things.Modifiers;

[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class ThingModifier_Collection : ThingModifier
{
    // don't rename this field. XML defs depend on this name
    private readonly List<ThingModifier> modifiers = default!;

    public override float GetModifier(Thing thing)
    {
        if (modifiers is not [_, ..])
        {
            Logger.ConfigError($"{nameof(ThingModifier_Collection)} has no modifiers. This is likely a mistake in the XML definition. Returning 1 as the modifier value.");
            return 1f;
        }
        float modifier = 1f;
        foreach (ThingModifier thingModifier in modifiers)
        {
            modifier *= thingModifier.GetModifier(thing);
        }
        return modifier;
    }
}
