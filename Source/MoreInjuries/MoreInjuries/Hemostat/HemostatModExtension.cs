using Verse;

namespace MoreInjuries.Hemostat;

// members initialized via XML defs
public class HemostatModExtension : DefModExtension
{
    // don't rename this field. XML defs depend on this name
    private readonly float _coagulationMultiplier;

    // don't rename this field. XML defs depend on this name
    private readonly int _applyTime;

    public float CoagulationMultiplier => _coagulationMultiplier;

    public int ApplyTime => _applyTime;
}
