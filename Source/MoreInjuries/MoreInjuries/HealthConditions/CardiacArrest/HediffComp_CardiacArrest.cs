using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.CardiacArrest;

public class HediffComp_CardiacArrest : HediffComp
{
    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        if (ModLister.BiotechInstalled && parent.pawn.health.hediffSet.HasHediff(HediffDefOf.Deathrest))
        {
            // immediate self-removal if the pawn is deathresting
            parent.pawn.health.RemoveHediff(parent);
        }
        base.CompPostPostAdd(dinfo);
    }
}
