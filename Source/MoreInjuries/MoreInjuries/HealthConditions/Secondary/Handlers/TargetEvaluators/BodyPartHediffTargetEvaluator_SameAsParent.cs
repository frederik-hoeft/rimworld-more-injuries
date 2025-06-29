using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;

public sealed class BodyPartHediffTargetEvaluator_SameAsParent : BodyPartHediffTargetEvaluator
{
    public override BodyPartRecord? GetTargetBodyPart(HediffComp comp, HediffCompHandler_SecondaryCondition handler) => comp.parent.Part;
}