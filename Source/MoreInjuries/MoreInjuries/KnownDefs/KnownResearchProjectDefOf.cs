using RimWorld;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.KnownDefs;

[DefOf]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Cannot encapsulate DefOf fields in properties, required for reflection. Name must match XML def name.")]
public static class KnownResearchProjectDefOf
{
    public static ResearchProjectDef AdvancedFirstAid = null!;
    public static ResearchProjectDef AdvancedThoracicSurgery = null!;
    public static ResearchProjectDef BasicAnatomy = null!;
    public static ResearchProjectDef BasicFirstAid = null!;
    public static ResearchProjectDef EmergencyMedicine = null!;
    public static ResearchProjectDef EpinephrineSynthesis = null!;
}