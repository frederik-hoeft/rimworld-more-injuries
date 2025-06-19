using MoreInjuries.HealthConditions.Secondary;
using MoreInjuries.HealthConditions.Secondary.Handlers;
using Verse;

namespace MoreInjuries.HealthConditions.Hypothermia.Secondary;

public sealed class HediffCompHandler_SecondaryCondition_CardiacArrest : HediffCompHandler_SecondaryCondition_TargetsBodyPart
{
    public override bool ShouldSkip(HediffComp_SecondaryCondition comp, float severityAdjustment) => base.ShouldSkip(comp, severityAdjustment)
        // this worker runs for anything that can get hypothermia, but we only want to apply cardiac arrest to pawns that actually support it
        || !comp.parent.pawn.HasComp<MoreInjuryComp>();
}
