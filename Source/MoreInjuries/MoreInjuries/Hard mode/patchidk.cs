using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MoreInjuries;

[StaticConstructorOnStartup]
public class patchidk
{
    static patchidk()
    {
        if (MoreInjuriesMod.Settings.advancedShock)
        {
            if (HediffDefOf.BloodLoss.comps == null)
            {
                HediffDefOf.BloodLoss.comps = new List<HediffCompProperties>();
            }
            HediffDefOf.BloodLoss.comps.Add(new HediffCompProperties { compClass = typeof(ShockMakerComp) });

            HediffDefOf.BloodLoss.hediffClass = typeof(HediffWithComps);
        }
    }
}
