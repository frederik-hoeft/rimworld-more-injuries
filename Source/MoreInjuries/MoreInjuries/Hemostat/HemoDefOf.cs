using RimWorld;
using Verse;

namespace MoreInjuries.Hemostat;

// loaded from XML defs
// TODO: ensure XML defs can initialize get-only members (was public static field)
[DefOf]
public class HemoDefOf : DefOf
{
    public static HediffDef HemostatHediffDef { get; } = null!;

    public static JobDef ApplyHemostatJob { get; } = null!;
}