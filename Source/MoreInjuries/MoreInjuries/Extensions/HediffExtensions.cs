using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.Extensions;

public static class HediffExtensions
{
    public static bool IsOnBodyPartOrChildren(this Hediff hediff, BodyPartRecord bodyPart)
    {
        for (BodyPartRecord part = hediff.Part; part is not null; part = part.parent)
        {
            if (part == bodyPart)
            {
                return true;
            }
        }
        return false;
    }

    public static bool TryFindMatchingBodyPart(this Hediff hediff, HashSet<BodyPartRecord> bodyParts, [NotNullWhen(returnValue: true)] out BodyPartRecord? matchingPart)
    {
        for (BodyPartRecord part = hediff.Part; part is not null; part = part.parent)
        {
            if (bodyParts.Contains(part))
            {
                matchingPart = part;
                return true;
            }
        }
        matchingPart = null;
        return false;
    }

    public static bool TryFindMatchingBodyPart<T>(this Hediff hediff, Dictionary<BodyPartRecord, T> bodyParts, [NotNullWhen(returnValue: true)] out BodyPartRecord? matchingPart, [MaybeNullWhen(returnValue: false)] out T matchingPartValue)
    {
        for (BodyPartRecord part = hediff.Part; part is not null; part = part.parent)
        {
            if (bodyParts.TryGetValue(part, out matchingPartValue))
            {
                matchingPart = part;
                return true;
            }
        }
        matchingPart = null;
        matchingPartValue = default;
        return false;
    }
}
