using Verse;

namespace MoreInjuries.HealthConditions.HypovolemicShock;

public sealed class HediffCompProperties_ShockMaker : HediffCompProperties
{
    public HediffCompProperties_ShockMaker() => compClass = typeof(HediffComp_ShockMaker);
}
