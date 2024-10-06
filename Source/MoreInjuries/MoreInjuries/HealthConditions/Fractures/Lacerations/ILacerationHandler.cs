using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Fractures.Lacerations;

internal interface ILacerationHandler
{
    HashSet<BodyPartDef>? TargetDefs { get; }

    IEnumerable<BodyPartRecord> GetTargets(Pawn patient, BodyPartRecord fracture);
}
