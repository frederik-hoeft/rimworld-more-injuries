using MoreInjuries.HealthConditions.Secondary;
using MoreInjuries.HealthConditions.Secondary.Handlers;
using Verse;

namespace MoreInjuries.HealthConditions;

public class HediffComp_SeverityPerDay_WithModifiers : HediffComp_SeverityPerDay, IHediffCompHandler
{
    public override float SeverityChangePerDay()
    {
        float baseSeverityChange = base.SeverityChangePerDay();
        if (parent.def.GetModExtension<HediffModifier_SeverityModifiers_ModExtension>() is { } downstream)
        {
            baseSeverityChange = downstream.ApplyTo(baseSeverityChange, parent, this);
        }
        return baseSeverityChange;
    }
}
