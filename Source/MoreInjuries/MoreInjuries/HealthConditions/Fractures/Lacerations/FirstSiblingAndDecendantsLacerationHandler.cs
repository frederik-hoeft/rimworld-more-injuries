﻿using MoreInjuries.Extensions;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Fractures.Lacerations;

internal class FirstSiblingAndDecendantsLacerationHandler(HashSet<BodyPartDef> targets) : ILacerationHandler
{
    public HashSet<BodyPartDef>? TargetDefs { get; } = targets;

    public IEnumerable<BodyPartRecord> GetTargets(Pawn patient, BodyPartRecord fracture)
    {
        HashSet<BodyPartDef> targets = TargetDefs!;
        foreach (BodyPartRecord sibling in fracture.parent.GetNonMissingDirectChildParts(patient))
        {
            if (targets.Contains(sibling.def))
            {
                yield return sibling;
                foreach (BodyPartRecord child in sibling.GetDescendants(patient))
                {
                    if (targets.Contains(child.def))
                    {
                        yield return child;
                    }
                }
                break;
            }
        }
    }
}
