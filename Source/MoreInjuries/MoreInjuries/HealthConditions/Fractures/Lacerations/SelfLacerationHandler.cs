using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Fractures.Lacerations;

internal class SelfLacerationHandler : ILacerationHandler
{
    public HashSet<BodyPartDef>? TargetDefs => null;

    public IEnumerable<BodyPartRecord> GetTargets(Pawn patient, BodyPartRecord fracture)
    {
        yield return fracture;
    }
}
