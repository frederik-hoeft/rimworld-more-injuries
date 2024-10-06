using MoreInjuries.Extensions;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.Fractures.Lacerations;

internal class SiblingsAndDecendantsLacerationHandler(HashSet<BodyPartDef>? targets = null) : ILacerationHandler
{
    public HashSet<BodyPartDef>? TargetDefs { get; } = targets;

    public IEnumerable<BodyPartRecord> GetTargets(Pawn patient, BodyPartRecord fracture)
    {
        HashSet<BodyPartDef>? targets = TargetDefs;
        return fracture.GetSiblingsAndDecendants(patient)
            .Where(bodyPart =>
                targets is null && !bodyPart.def.IsSolidInDefinition_Debug
                || targets is not null && targets.Contains(bodyPart.def));
    }
}
