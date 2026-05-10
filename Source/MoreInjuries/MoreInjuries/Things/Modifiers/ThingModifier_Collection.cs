using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.Things.Modifiers;

[XmlBindable]
public sealed partial class ThingModifier_Collection : ThingModifier
{
    [XmlBinding("modifiers")]
    public partial IReadOnlyList<ThingModifier> Modifiers { get; }

    public override float GetModifier(Thing thing)
    {
        float modifier = 1f;
        foreach (ThingModifier thingModifier in Modifiers)
        {
            modifier *= thingModifier.GetModifier(thing);
        }
        return modifier;
    }
}
