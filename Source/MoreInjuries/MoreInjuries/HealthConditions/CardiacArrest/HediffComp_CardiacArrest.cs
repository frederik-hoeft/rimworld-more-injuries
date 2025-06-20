using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.CardiacArrest;

// TODO: use this comp as a primary component for cardiac arrest, and remove lethality with severity == 1 => death (replace with hypoxia as secondary effect/primary cause of death)
// (this would also give us the much-requested integration with DeathRattle, which currently doesn't fire when cardiac arrest instant-kills the pawn on severity == 1)
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
