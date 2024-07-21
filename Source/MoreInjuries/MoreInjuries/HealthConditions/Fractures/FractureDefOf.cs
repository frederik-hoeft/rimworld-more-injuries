using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Fractures;

[DefOf]
public class FractureDefOf : DefOf
{
    public static HediffDef FractureHealing { get; } = null!;

    public static HediffDef Fracture { get; } = null!;

    public static SoundDef MoreInjuries_BoneSnap { get; } = null!;
}
