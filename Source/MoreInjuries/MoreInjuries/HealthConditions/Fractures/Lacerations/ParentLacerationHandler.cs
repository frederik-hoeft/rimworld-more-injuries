using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Fractures.Lacerations;

internal class ParentLacerationHandler() : ILacerationHandler
{
    public HashSet<BodyPartDef>? TargetDefs => null;

    public IEnumerable<BodyPartRecord> GetTargets(Pawn patient, BodyPartRecord fracture) => [fracture.parent];
}
