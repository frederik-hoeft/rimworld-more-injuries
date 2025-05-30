using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers;

public abstract class BodyPartHediffTargetEvaluator
{
    public abstract BodyPartRecord? GetTargetBodyPart(HediffComp comp, HediffCompHandler_SecondaryCondition_TargetsBodyPart handler);
}
