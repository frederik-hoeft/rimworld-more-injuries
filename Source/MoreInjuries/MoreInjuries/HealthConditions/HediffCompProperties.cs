using Verse;

namespace MoreInjuries.HealthConditions;

public abstract class HediffCompProperties<T> : HediffCompProperties where T : HediffComp
{
    protected HediffCompProperties() => compClass = typeof(T);
}
