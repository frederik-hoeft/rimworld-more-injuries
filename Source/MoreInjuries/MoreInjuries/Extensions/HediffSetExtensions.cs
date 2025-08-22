using System.Collections.Generic;
using Verse;

namespace MoreInjuries.Extensions;

public static class HediffSetExtensions
{
    public static IEnumerable<BodyPartRecord> GetNonMissingPartsOfType(this HediffSet hediffSet, BodyPartDef bodyPartDef)
    {
        List<BodyPartRecord> allPartsList = hediffSet.pawn.def.race.body.AllParts;
        for (int i = 0; i < allPartsList.Count; ++i)
        {
            BodyPartRecord part = allPartsList[i];
            if (part.def == bodyPartDef && !hediffSet.PartIsMissing(part))
            {
                yield return part;
            }
        }
    }

    public static bool TryGetFirstHediffMatchingPart(this HediffSet hediffSet, BodyPartRecord part, HediffDef hediffDef, [NotNullWhen(returnValue: true)] out Hediff? hediff)
    {
        List<Hediff> hediffs = hediffSet.hediffs;
        for (int i = 0; i < hediffs.Count; ++i)
        {
            hediff = hediffs[i];
            if (hediff.def == hediffDef && hediff.Part == part)
            {
                return true;
            }
        }
        hediff = null;
        return false;
    }

    public static IEnumerable<Hediff> HediffsMatchingPart(this HediffSet hediffSet, BodyPartRecord part, HediffDef hediffDef)
    {
        List<Hediff> hediffs = hediffSet.hediffs;
        for (int i = 0; i < hediffs.Count; ++i)
        {
            Hediff hediff = hediffs[i];
            if (hediff.def == hediffDef && hediff.Part == part)
            {
                yield return hediff;
            }
        }
    }

    public static IEnumerable<Hediff> HediffsMatchingPart(this HediffSet hediffSet, BodyPartRecord part)
    {
        List<Hediff> hediffs = hediffSet.hediffs;
        for (int i = 0; i < hediffs.Count; ++i)
        {
            Hediff hediff = hediffs[i];
            if (hediff.Part == part)
            {
                yield return hediff;
            }
        }
    }

    public static void RemoveHediffsMatchingPartOrChildren(this HediffSet hediffSet, BodyPartRecord part, Hediff? exception = null)
    {
        for (int i = hediffSet.hediffs.Count - 1; i >= 0; --i)
        {
            Hediff hediff = hediffSet.hediffs[i];
            if (hediff != exception && hediff.IsOnBodyPartOrChildren(part))
            {
                hediffSet.hediffs.RemoveAt(i);
            }
        }
    }
}
