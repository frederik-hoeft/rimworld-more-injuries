using System.Linq;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.AirwayBurns;

public class DamageWorker_SetOnFire : DamageWorker_Flame
{
    public override DamageResult Apply(DamageInfo dinfo, Thing victim)
    {
        DamageResult result = base.Apply(dinfo, victim);
        if (MoreInjuriesMod.Settings.enableFireInhalation
            && victim is Pawn p
            && Rand.Chance(0.125f * p.GetStatValue(StatDefOf.ToxicResistance))
            && p.IsBurning())
        {
            foreach (BodyPartRecord? lung in p.health.hediffSet.GetNotMissingParts().Where(x => x.def.defName == BodyPartDefOf.Lung.defName))
            {
                Hediff burnHediff = HediffMaker.MakeHediff(DamageDefOf.Burn.hediff, p, lung);
                burnHediff.Severity = Rand.Range(dinfo.Amount, dinfo.Amount * 2f);
                p.health.AddHediff(burnHediff, lung);
            }
        }
        return result;
    }
}
