using System.Linq;
using Verse;

namespace MoreInjuries.Extensions;

public static class Hediff_InjuryExtensions
{
    public static bool IsOnBodyPartOrChildren(this Hediff_Injury injury, BodyPartRecord bodyPart) => injury.Part == bodyPart
        || bodyPart.parts is { Count: > 0 }
            && (bodyPart.parts.Contains(injury.Part)
                || bodyPart.parts.Any(part => part.parts?.Contains(injury.Part) is true));
}
