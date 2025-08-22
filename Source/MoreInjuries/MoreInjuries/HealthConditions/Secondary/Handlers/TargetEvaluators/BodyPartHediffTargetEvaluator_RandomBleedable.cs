using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;

public class BodyPartHediffTargetEvaluator_RandomBleedable : BodyPartHediffTargetEvaluator_Random
{
    protected override bool IncludeBodyPart(BodyPartRecord bodyPart, Pawn pawn) =>
        // include only body parts that have a bleed rate greater than 0
        base.IncludeBodyPart(bodyPart, pawn)
        && bodyPart.def.bleedRate > Mathf.Epsilon
        && !bodyPart.def.IsSolid(bodyPart, pawn.health.hediffSet.hediffs)
        && bodyPart.coverage > 0f;
}
