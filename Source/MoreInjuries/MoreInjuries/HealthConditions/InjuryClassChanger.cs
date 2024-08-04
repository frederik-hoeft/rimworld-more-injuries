using MoreInjuries.HealthConditions.Amputations;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions;

[StaticConstructorOnStartup]
public class InjuryClassChanger
{
    static InjuryClassChanger()
    {
        foreach (HediffDef hediffdef in DefDatabase<HediffDef>.AllDefsListForReading.Where(hediffDef => hediffDef.hediffClass == typeof(Hediff_Injury)))
        {
            hediffdef.hediffClass = typeof(BetterInjury);
        }
        foreach (HediffDef hediffdef in DefDatabase<HediffDef>.AllDefsListForReading.Where(hediffDef => hediffDef.hediffClass == typeof(Hediff_MissingPart)))
        {
            hediffdef.hediffClass = typeof(AmputationHediff);
        }
    }
}
