﻿using System.Collections.Generic;
using System.Linq;
using MoreInjuries.HealthConditions.HearingLoss;
using MoreInjuries.HealthConditions.HeavyBleeding.Overrides;
using MoreInjuries.HealthConditions.HypovolemicShock;
using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions;

[StaticConstructorOnStartup]
public static class HealthCondition_Initializer
{
    static HealthCondition_Initializer()
    {
        // Note: not entirely sure what this is trying to achieve...
        KnownBodyPartDefOf.Skull.hitPoints = 35;

        // inject hearing loss hooks into all gun defs
        IEnumerable<ThingDef> guns = DefDatabase<ThingDef>.AllDefsListForReading
            .Where(static def => def is 
            { 
                weaponClasses.Count: > 0, 
                Verbs.Count: > 0 
            } && def.Verbs.Any(static verb => verb.range > 0));
        foreach (ThingDef def in guns)
        {
            def.comps ??= [];
            def.comps.Add(new CompProperties
            {
                compClass = typeof(HearingLossComp)
            });
        }

        // hook into all injury and missing part hediffs
        foreach (HediffDef hediffdef in DefDatabase<HediffDef>.AllDefsListForReading.Where(static hediffDef => hediffDef.hediffClass == typeof(Hediff_Injury)))
        {
            hediffdef.hediffClass = typeof(BetterInjury);
        }
        foreach (HediffDef hediffdef in DefDatabase<HediffDef>.AllDefsListForReading.Where(static hediffDef => hediffDef.hediffClass == typeof(Hediff_MissingPart)))
        {
            hediffdef.hediffClass = typeof(BetterMissingPart);
        }
    }
}

