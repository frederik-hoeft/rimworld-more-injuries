using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public sealed class HediffModifier_DisallowHidden : SecondaryHediffModifier
{
    public override float GetModifier(Hediff hediff, HediffCompHandler compHandler)
    {
        if (hediff.Visible)
        {
            // if the hediff is visible, we allow it
            return 1f;
        }
        // if the hediff is hidden, so return 0-factor
        return 0f;
    }
}