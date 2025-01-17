using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.HealthConditions;

// members initialized via XML defs
[SuppressMessage("Style", "IDE0032:Use auto property", Justification = "Cannot use auto property for XML defs")]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Comply with default RimWorld XML naming convention")]
public class ReusabilityProps_ModExtension : DefModExtension
{
    // don't rename this field. XML defs depend on this name
    private readonly float destroyChance = default;

    public float DestroyChance => destroyChance;
}