using Verse;

namespace MoreInjuries.Extensions;

public static class Hediff_InjuryExtensions
{
    public static bool IsOnBodyPartOrChildren(this Hediff_Injury injury, BodyPartRecord bodyPart)
    {
        for (; bodyPart is not null; bodyPart = bodyPart.parent)
        {
            if (injury.Part == bodyPart)
            {
                return true;
            }
        }
        return false;
    }
}
