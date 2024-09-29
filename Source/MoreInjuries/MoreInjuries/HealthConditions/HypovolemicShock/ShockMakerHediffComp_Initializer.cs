using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.HypovolemicShock;

[StaticConstructorOnStartup]
internal class ShockMakerHediffComp_Initializer
{
    static ShockMakerHediffComp_Initializer()
    {
        // TODO: this implementation requires a full game restart to take effect (not optimal)
        if (MoreInjuriesMod.Settings.EnableHypovolemicShock)
        {
            HediffDefOf.BloodLoss.comps ??= [];
            HediffDefOf.BloodLoss.comps.Add(new HediffCompProperties
            {
                compClass = typeof(ShockMakerHediffComp)
            });
            HediffDefOf.BloodLoss.hediffClass = typeof(HediffWithComps);
        }
    }
}
