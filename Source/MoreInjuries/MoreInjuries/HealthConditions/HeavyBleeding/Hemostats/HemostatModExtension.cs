using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Hemostats;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = "Cannot use auto property for XML defs")]
public class HemostatModExtension : DefModExtension
{
    // don't rename this field. XML defs depend on this name
    private readonly float _coagulationMultiplier = default;

    // don't rename this field. XML defs depend on this name
    private readonly int _applyTime = default;

    public float CoagulationMultiplier => _coagulationMultiplier;

    public int ApplyTime => _applyTime;
}
