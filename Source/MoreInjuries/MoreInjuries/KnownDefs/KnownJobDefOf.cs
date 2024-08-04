using RimWorld;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.KnownDefs;

[DefOf]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Cannot encapsulate DefOf fields in properties, required for reflection. Name must match XML def name.")]
public static class KnownJobDefOf
{
    public static JobDef ClearAirway = null!;

    public static JobDef PerformCpr = null!;

    public static JobDef ApplySplint = null!;

    public static JobDef ApplyTourniquet = null!;

    public static JobDef ApplyHemostat = null!;

    public static JobDef ProvideFirstAid = null!;
}
