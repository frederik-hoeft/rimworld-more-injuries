using MoreInjuries.Extensions;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.Fractures.Lacerations;

internal class SelfAndDescendantsLacerationHandler(BodyPartDef[]? targets = null) : ILacerationHandler
{
    public BodyPartDef[]? TargetDefs { get; } = targets;

    public IEnumerable<BodyPartRecord> GetTargets(Pawn patient, BodyPartRecord fracture)
    {
        BodyPartDef[]? targets = TargetDefs;
        yield return fracture;
        foreach (BodyPartRecord child in fracture.GetDescendants(patient))
        {
            if (targets is null && !child.def.IsSolidInDefinition_Debug || targets is not null && targets.Contains(child.def))
            {
                yield return child;
            }
        }
    }
}
