using Verse;

namespace MoreInjuries.Things.Modifiers;

public abstract class ThingModifier
{
    public abstract float GetModifier(Thing thing);
}
