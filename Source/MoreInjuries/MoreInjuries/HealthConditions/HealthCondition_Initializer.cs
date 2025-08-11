using System.Linq;
using MoreInjuries.Defs.WellKnown;
using MoreInjuries.HealthConditions.HeavyBleeding.Overrides;
using Verse;

namespace MoreInjuries.HealthConditions;

[StaticConstructorOnStartup]
public static class HealthCondition_Initializer
{
    static HealthCondition_Initializer()
    {
        // Note: not entirely sure what this is trying to achieve... default is 25
        KnownBodyPartDefOf.Skull.hitPoints = 35;

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

