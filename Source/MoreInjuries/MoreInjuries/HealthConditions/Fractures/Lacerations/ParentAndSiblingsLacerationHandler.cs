using MoreInjuries.Extensions;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Fractures.Lacerations;

internal class ParentAndSiblingsLacerationHandler(HashSet<BodyPartDef>? targets = null) : ILacerationHandler
{
    public HashSet<BodyPartDef>? TargetDefs { get; } = targets;

    public IEnumerable<BodyPartRecord> GetTargets(Pawn patient, BodyPartRecord fracture)
    {
        HashSet<BodyPartDef>? targets = TargetDefs;
        if (targets is null && !fracture.parent.def.IsSolidInDefinition_Debug || targets is not null && targets.Contains(fracture.parent.def))
        {
            yield return fracture.parent;
        }
        foreach (BodyPartRecord sibling in fracture.parent.GetNonMissingDirectChildParts(patient))
        {
            if (targets is null && !sibling.def.IsSolidInDefinition_Debug || targets is not null && targets.Contains(sibling.def))
            {
                yield return sibling;
            }
        }
    }
}
