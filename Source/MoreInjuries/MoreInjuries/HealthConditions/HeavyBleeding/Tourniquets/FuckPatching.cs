using System.Collections.Generic;
using System.Linq;
using MoreInjuries.HealthConditions.HearingLoss;
using MoreInjuries.HealthConditions.HeavyBleeding.Hemostat;
using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

[StaticConstructorOnStartup]
public class FuckPatching
{
    // TODO: consolidate
    static FuckPatching()
    {
        KnownBodyPartDefOf.Skull.hitPoints = 35;

        BodyPartDefOf.Head.hitPoints = 20;

        ThingDefOf.Human.comps.Add(new CompProperties { compClass = typeof(TourniquetThingComp) });
        ThingDefOf.Human.comps.Add(new CompProperties { compClass = typeof(HemostatThingComp) });

        // TODO: should be moved to hearing loss
        if (MoreInjuriesMod.Settings.HearDMG)
        {
            IEnumerable<ThingDef> guns = DefDatabase<ThingDef>.AllDefsListForReading.Where(def => def.Verbs?.Any(verb => verb.range > 0) is true);
            foreach (ThingDef def in guns)
            {
                def.comps ??= [];
                def.comps.Add(new CompProperties 
                { 
                    compClass = typeof(HearingLossComp) 
                });
            }
        }

        MoreInjuriesMod.Settings.fuckYourFun = false;

        //JobDefOf.TendPatient.driverClass = typeof(JobDriver_TendPatient);
    }
}

