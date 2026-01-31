using MoreInjuries.Extensions;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.CardiacArrest;

public class HediffComp_CardiacArrest : HediffComp
{
    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        Pawn pawn = parent.pawn;

        // Remove if pawn has oxygen deficiency immunity (Deathless/Breathless genes)
        if (pawn.HasOxygenDeficiencyImmunity())
        {
            Logger.LogDebug($"Removing cardiac arrest from {pawn.Name} due to oxygen-deficiency immunity gene");
            pawn.health.RemoveHediff(parent);
            return;
        }

        // Remove if pawn is deathresting (Biotech integration)
        if (ModLister.BiotechInstalled && pawn.health.hediffSet.HasHediff(HediffDefOf.Deathrest))
        {
            // immediate self-removal if the pawn is deathresting
            pawn.health.RemoveHediff(parent);
            return;
        }

        base.CompPostPostAdd(dinfo);
    }
}
