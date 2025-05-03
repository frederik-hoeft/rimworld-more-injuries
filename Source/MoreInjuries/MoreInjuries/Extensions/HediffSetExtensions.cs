using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.Extensions;

public static class HediffSetExtensions
{
    public static bool TryGetFirstHediffMatchingPart(this HediffSet hediffSet, BodyPartRecord part, HediffDef hediffDef, [NotNullWhen(returnValue: true)] out Hediff? hediff)
    {
        for (int index = 0; index < hediffSet.hediffs.Count; ++index)
        {
            hediff = hediffSet.hediffs[index];
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
        for (int index = 0; index < hediffSet.hediffs.Count; ++index)
        {
            Hediff hediff = hediffSet.hediffs[index];
            if (hediff.def == hediffDef && hediff.Part == part)
            {
                yield return hediff;
            }
        }
    }

    public static IEnumerable<Hediff> HediffsMatchingPart(this HediffSet hediffSet, BodyPartRecord part)
    {
        for (int index = 0; index < hediffSet.hediffs.Count; ++index)
        {
            Hediff hediff = hediffSet.hediffs[index];
            if (hediff.Part == part)
            {
                yield return hediff;
            }
        }
    }

    public static void RemoveHediffsMatchingPartOrChildren(this HediffSet hediffSet, BodyPartRecord part, Hediff? exception = null)
    {
        for (int index = hediffSet.hediffs.Count - 1; index >= 0; --index)
        {
            Hediff hediff = hediffSet.hediffs[index];
            if (hediff != exception && hediff.IsOnBodyPartOrChildren(part))
            {
                hediffSet.hediffs.RemoveAt(index);
            }
        }
    }
}
