﻿using MoreInjuries.Extensions;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Fractures.Lacerations;

internal class SiblingLacerationHandler(HashSet<BodyPartDef>? targets = null) : ILacerationHandler
{
    public HashSet<BodyPartDef>? TargetDefs { get; } = targets;

    public IEnumerable<BodyPartRecord> GetTargets(Pawn patient, BodyPartRecord fracture)
    {
        HashSet<BodyPartDef>? targets = TargetDefs;
        foreach (BodyPartRecord sibling in fracture.parent.GetNonMissingDirectChildParts(patient))
        {
            if (targets is null && !sibling.def.IsSolidInDefinition_Debug || targets is not null && targets.Contains(sibling.def))
            {
                yield return sibling;
            }
        }
    }
}
