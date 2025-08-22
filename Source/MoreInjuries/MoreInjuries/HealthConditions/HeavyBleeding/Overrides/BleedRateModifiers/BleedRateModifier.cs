using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Overrides.BleedRateModifiers;

public abstract class BleedRateModifier
{
    public abstract float GetModifierFor(Hediff hediff, HediffWithComps bleedingHediff);
}