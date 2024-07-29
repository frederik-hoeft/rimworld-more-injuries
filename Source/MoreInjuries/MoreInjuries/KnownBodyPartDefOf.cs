using RimWorld;
using Verse;

namespace MoreInjuries;

[DefOf]
public static class KnownBodyPartDefOf
{
    // defs not covered by BodyPartDefOf for some reason
    public static BodyPartDef Stomach = null!;
    public static BodyPartDef Liver = null!;
    public static BodyPartDef Kidney = null!;
    // added by MoreInjuries
    public static BodyPartDef SmallIntestine = null!;
    public static BodyPartDef LargeIntestine = null!;
}