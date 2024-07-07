using System.Linq;
using RimWorld;
using Verse;

namespace MoreInjuries;

[StaticConstructorOnStartup]
public class Patcher69
{
    static Patcher69()
    {
        DamageDefOf.Flame.workerClass = typeof(DamageWorker_SetOnFire);
    }
}

public class DamageWorker_SetOnFire : DamageWorker_Flame
{
    public override DamageResult Apply(DamageInfo dinfo, Thing victim)
    {
        DamageResult result = base.Apply(dinfo, victim);
        if (MoreInjuriesMod.Settings.fireInhalation)
        {
            if (victim is Pawn p && (Rand.Chance(0.125f * p.GetStatValue(StatDefOf.ToxicResistance))))
            {
                if (FireUtility.IsBurning(p))
                {
                    foreach (BodyPartRecord? lung in p.health.hediffSet.GetNotMissingParts().Where(x => x.def.defName == "Lung"))
                    {
                        Hediff HediffBurn = HediffMaker.MakeHediff(HediffDefOf.Burn, p, lung);

                        HediffBurn.Severity = Rand.Range(dinfo.Amount, dinfo.Amount * 2f);

                        p.health.AddHediff(HediffBurn, lung);
                    }
                }
            }
        }

        return result;
    }
}
