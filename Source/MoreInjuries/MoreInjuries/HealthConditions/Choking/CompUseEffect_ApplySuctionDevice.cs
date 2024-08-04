using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Choking;

public class CompUseEffect_ApplySuctionDevice : CompUseEffect
{
    public override void DoEffect(Pawn usedBy) =>
        usedBy.health.hediffSet.hediffs.RemoveAll(hediff => hediff.def == KnownHediffDefOf.ChokingOnBlood);
}
