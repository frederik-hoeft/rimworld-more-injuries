using RimWorld;
using Verse;

namespace MoreInjuries.HypovolemicShock;

[StaticConstructorOnStartup]
internal class Bootstrap_Shock
{
    static Bootstrap_Shock()
    {
        // TODO: this implementation requires a full game restart to take effect (not optimal)
        if (MoreInjuriesMod.Settings.HypovolemicShockEnabled)
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
