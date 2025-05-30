using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers;

public class BodyPartHediffTargetEvaluator_RandomBleedable : BodyPartHediffTargetEvaluator_Random
{
    protected override bool IncludeBodyPart(BodyPartRecord bodyPart)
    {
        // include only body parts that have a bleed rate greater than 0
        return base.IncludeBodyPart(bodyPart) && bodyPart.def.bleedRate > 0f;
    }
}
