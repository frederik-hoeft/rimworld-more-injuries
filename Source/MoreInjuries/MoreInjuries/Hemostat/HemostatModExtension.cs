using Verse;

namespace MoreInjuries.Hemostat;

// members initialized via XML defs
public class HemostatModExtension : DefModExtension
{
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
    // don't rename this field. XML defs depend on this name
    private readonly float _coagulationMultiplier;

    // don't rename this field. XML defs depend on this name
    private readonly int _applyTime;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

    public float CoagulationMultiplier => _coagulationMultiplier;

    public int ApplyTime => _applyTime;
}
