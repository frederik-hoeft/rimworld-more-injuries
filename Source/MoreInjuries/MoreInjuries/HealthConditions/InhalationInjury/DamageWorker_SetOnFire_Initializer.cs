using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.InhalationInjury;

[StaticConstructorOnStartup]
public class DamageWorker_SetOnFire_Initializer
{
    static DamageWorker_SetOnFire_Initializer()
    {
        DamageDefOf.Flame.workerClass = typeof(DamageWorker_SetOnFire);
    }
}
