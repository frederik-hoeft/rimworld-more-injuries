using RimWorld;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.KnownDefs;

[DefOf]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Cannot encapsulate DefOf fields in properties, required for reflection. Name must match XML def name.")]
public static class KnownJobDefOf
{
    public static JobDef UseSuctionDevice = null!;

    public static JobDef PerformCpr = null!;

    public static JobDef UseSplint = null!;

    public static JobDef UseTourniquet = null!;

    public static JobDef RemoveTourniquet = null!;

    public static JobDef UseHemostaticAgent = null!;

    public static JobDef UseDefibrillator = null!;

    public static JobDef UseBandage = null!;

    public static JobDef UseEpinephrine = null!;
}
