using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;

public abstract class HediffMakerProperties
{
    public abstract HediffMakerDef GetHediffMakerDef(HediffComp parentComp, HediffCompHandler_SecondaryCondition handler, BodyPartRecord? targetBodyPart);
}
