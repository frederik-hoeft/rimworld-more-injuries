using MoreInjuries.Defs.WellKnown;
using MoreInjuries.HealthConditions.HypovolemicShock;
using Verse;

namespace MoreInjuries.Integrations.Biotech;

public class HediffComp_DeathrestIntegration : HediffComp
{
    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        base.CompPostPostAdd(dinfo);

        Pawn pawn = parent.pawn;
        // the pawn is now deathresting, so remove all progressing conditions
        if (pawn.health.hediffSet.TryGetHediff(KnownHediffDefOf.HypovolemicShock, out Hediff? hypovolemicShock))
        {
            if (hypovolemicShock.TryGetComp(out HediffComp_Shock comp))
            {
                // force the hypovolemic shock to recover over time
                comp.FixedNow = true;
            }
            else
            {
                // no idea what happened, just remove the hediff
                Logger.Warning($"Hypovolemic shock on {pawn.Name} has no shock comp, removing. This should never happen. Please report this issue to the mod author.");
                pawn.health.RemoveHediff(hypovolemicShock);
            }
        }
        // just remove any cardiac arrests
        if (pawn.health.hediffSet.TryGetHediff(KnownHediffDefOf.CardiacArrest, out Hediff? cardiacArrest))
        {
            pawn.health.RemoveHediff(cardiacArrest);
        }
    }
}
