using Verse;

namespace MoreInjuries.HealthConditions;

public class HediffCompProperties_SeverityPerDay_WithModifiers : HediffCompProperties_SeverityPerDay
{
    public HediffCompProperties_SeverityPerDay_WithModifiers() => compClass = typeof(HediffComp_SeverityPerDay_WithModifiers);
}
