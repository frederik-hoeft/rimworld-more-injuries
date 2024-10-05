using System.Collections.Generic;
using Verse;

namespace MoreInjuries.Extensions;

public static class BodyPartRecordExtensions
{
    public static IEnumerable<BodyPartRecord> GetSiblingsAndDecendants(this BodyPartRecord bodyPart, Pawn patient)
    {
        foreach (BodyPartRecord sibling in bodyPart.parent.GetNonMissingDirectChildParts(patient))
        {
            yield return sibling;
            foreach (BodyPartRecord child in sibling.GetDescendants(patient))
            {
                yield return child;
            }
        }
    }

    public static IEnumerable<BodyPartRecord> GetDescendants(this BodyPartRecord bodyPart, Pawn patient)
    {
        foreach (BodyPartRecord child in bodyPart.GetNonMissingDirectChildParts(patient))
        {
            yield return child;
            foreach (BodyPartRecord grandchild in child.GetDescendants(patient))
            {
                yield return grandchild;
            }
        }
    }

    public static IEnumerable<BodyPartRecord> GetNonMissingDirectChildParts(this BodyPartRecord bodyPart, Pawn patient)
    {
        foreach (BodyPartRecord child in bodyPart.parts)
        {
            if (!patient.health.hediffSet.PartIsMissing(child))
            {
                yield return child;
            }
        }
    }
}