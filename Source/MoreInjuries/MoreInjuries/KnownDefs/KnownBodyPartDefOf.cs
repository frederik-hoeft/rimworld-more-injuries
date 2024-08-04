using RimWorld;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.KnownDefs;

[DefOf]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Cannot encapsulate DefOf fields in properties, required for reflection. Name must match XML def name.")]
public static class KnownBodyPartDefOf
{
    // defs not covered by BodyPartDefOf for some reason
    public static BodyPartDef Stomach = null!;
    public static BodyPartDef Liver = null!;
    public static BodyPartDef Kidney = null!;
    public static BodyPartDef Skull = null!;
    public static BodyPartDef Neck = null!;
    public static BodyPartDef Ear = null!;
    // added by MoreInjuries
    public static BodyPartDef SmallIntestine = null!;
    public static BodyPartDef LargeIntestine = null!;
    public static BodyPartDef SpinalCord = null!;
}
