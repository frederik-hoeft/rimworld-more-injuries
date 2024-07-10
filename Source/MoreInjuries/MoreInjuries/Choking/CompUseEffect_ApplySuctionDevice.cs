using RimWorld;
using Verse;

namespace MoreInjuries.Choking;

public class CompUseEffect_ApplySuctionDevice : CompUseEffect
{
    public override void DoEffect(Pawn usedBy) =>
        usedBy.health.hediffSet.hediffs.RemoveAll(hediff => hediff.def == MoreInjuriesHediffDefOf.ChokingOnBlood);
}
