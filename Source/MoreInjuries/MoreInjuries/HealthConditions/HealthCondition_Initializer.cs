using System.Collections.Generic;
using System.Linq;
using MoreInjuries.HealthConditions.HearingLoss;
using MoreInjuries.HealthConditions.HeavyBleeding.HemostaticAgents;
using MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;
using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions;

[StaticConstructorOnStartup]
public static class HealthCondition_Initializer
{
    static HealthCondition_Initializer()
    {
        KnownBodyPartDefOf.Skull.hitPoints = 35;

        BodyPartDefOf.Head.hitPoints = 20;

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
}

