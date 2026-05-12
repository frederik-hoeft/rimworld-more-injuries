using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public sealed class HediffModifier_ApplyInboundCauses : SecondaryHediffModifier
{
    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler)
    {
        if (hediff.def.GetModExtension<HediffModifier_InboundCauseComposite_ModExtension>() is { } downstream)
        {
            return downstream.GetModifier(hediff, compHandler);
        }
        return 1f;
    }
}
