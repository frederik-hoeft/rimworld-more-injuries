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
            throw new InvalidOperationException("ThingModifier_Collection must contain at least one modifier to be evaluated.");
        }
        float modifier = 1f;
        foreach (ThingModifier thingModifier in modifiers)
        {
            modifier *= thingModifier.GetModifier(thing);
        }
        return modifier;
    }
}
