using RimWorld;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.KnownDefs;

[DefOf]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Cannot encapsulate DefOf fields in properties, required for reflection. Name must match XML def name.")]
public static class KnownThingDefOf
{
    public static ThingDef Bandage = null!;

    public static ThingDef HemostaticAgent = null!;

    public static ThingDef Splint = null!;

    public static ThingDef SuctionDevice = null!;

    public static ThingDef Tourniquet = null!;

    public static ThingDef Defibrillator = null!;
}