using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.Things.Modifiers;

[XmlSerializable]
public sealed partial class ThingModifier_Collection : ThingModifier
{
    [XmlMember("modifiers")]
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
