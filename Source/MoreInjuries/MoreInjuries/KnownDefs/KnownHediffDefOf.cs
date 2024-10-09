using RimWorld;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.KnownDefs;

[DefOf]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Cannot encapsulate DefOf fields in properties, required for reflection. Name must match XML def name.")]
public static class KnownHediffDefOf
{
    // default defs not available through HediffDefOf
    public static HediffDef Bruise = null!;
    public static HediffDef Burn = null!;
    // added by MoreInjuries
    public static HediffDef AdrenalineRush = null!;
    public static HediffDef AirwayBlocked = null!;
    public static HediffDef Bandaged = null!;
    public static HediffDef BoneFragmentLaceration = null!; 
    public static HediffDef ChokingOnBlood = null!;
    public static HediffDef ChokingOnTourniquet = null!;
    public static HediffDef Concussion = null!;
    public static HediffDef Crush = null!;
    public static HediffDef EmpShutdown = null!;
    public static HediffDef Fracture = null!;
    public static HediffDef FractureHealing = null!;
    public static HediffDef HearingLoss = null!;
    public static HediffDef HemorrhagicStroke = null!;
    public static HediffDef HemostatApplied = null!;
    public static HediffDef HypovolemicShock = null!;
    public static HediffDef LungCollapse = null!;
    public static HediffDef OrganHypoxia = null!;
    public static HediffDef SpallFragmentCut = null!;
    public static HediffDef SpinalCordParalysis = null!;
    public static HediffDef StomachAcidBurn = null!;
    public static HediffDef TourniquetApplied = null!;
    public static HediffDef CardiacArrest = null!;
}