using RimWorld;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.KnownDefs;

[DefOf]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Cannot encapsulate DefOf fields in properties, required for reflection. Name must match XML def name.")]
public static class KnownSoundDefOf
{
    public static SoundDef BoneSnap = null!;
    public static SoundDef Choking = null!;
    public static SoundDef ChokingCoughMale = null!;
    public static SoundDef ChokingCoughFemale = null!;
    public static SoundDef Defibrillator = null!;
    public static SoundDef UseAutoinjector = null!;
    public static SoundDef UseSuctionDevice = null!;
    public static SoundDef PerformCpr = null!;
}
