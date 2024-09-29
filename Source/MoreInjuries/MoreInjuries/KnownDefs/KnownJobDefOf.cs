using RimWorld;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.KnownDefs;

[DefOf]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Cannot encapsulate DefOf fields in properties, required for reflection. Name must match XML def name.")]
public static class KnownJobDefOf
{
    public static JobDef ClearAirwayJob = null!;

    public static JobDef PerformCprJob = null!;

    public static JobDef ApplySplintJob = null!;

    public static JobDef ApplyTourniquetJob = null!;

    public static JobDef ApplyHemostatJob = null!;

    public static JobDef ProvideFirstAidJob = null!;
}
