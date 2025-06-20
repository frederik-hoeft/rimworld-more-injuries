using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;

public abstract class BodyPartHediffTargetEvaluator
{
    public abstract BodyPartRecord? GetTargetBodyPart(HediffComp parentComp, HediffCompHandler_SecondaryCondition handler);
}