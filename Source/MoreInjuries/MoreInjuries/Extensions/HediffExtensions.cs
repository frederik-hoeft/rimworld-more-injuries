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
}
