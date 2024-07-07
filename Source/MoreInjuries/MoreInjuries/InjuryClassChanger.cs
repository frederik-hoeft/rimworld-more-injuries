using Verse;

namespace MoreInjuries;

[StaticConstructorOnStartup]
public class InjuryClassChanger
{
    static InjuryClassChanger()
    {
        foreach (HediffDef hediffdef in DefDatabase<HediffDef>.AllDefsListForReading.FindAll(t => t.hediffClass == typeof(Hediff_Injury)))
        {
            hediffdef.hediffClass = typeof(BetterInjury);
        }
        foreach (HediffDef hediffdef in DefDatabase<HediffDef>.AllDefsListForReading.FindAll(t => t.hediffClass == typeof(Hediff_MissingPart)))
        {
            hediffdef.hediffClass = typeof(BetterPartMissing);
        }
    }
}
