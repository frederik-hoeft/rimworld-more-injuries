using MoreInjuries.BuildIntrinsics;
using RimWorld;
using Verse;

namespace MoreInjuries.Defs.WellKnown;

[DefOf]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_DEF_OF_REQUIRES_FIELD)]
public static class KnownRecipeDefOf
{
    public static RecipeDef ExtractWholeBloodBag = null!;
    public static RecipeDef RepairFracture = null!;
    public static RecipeDef SplintFracture = null!;
}
