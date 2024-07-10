using RimWorld;
using Verse;

namespace MoreInjuries.HypovolemicShock;

// loaded from XML defs
// TODO: ensure XML defs can initialize get-only members (was public static field)
[DefOf]
public class ShockDefOf
{
    public static HediffDef HypovolemicShock { get; } = null!;

    public static HediffDef OrganHypoxia { get; } = null!;
}
