using RimWorld;
using Verse;

namespace MoreInjuries.Jobs;

public class fixlung : CompUseEffect
{
    public override void DoEffect(Pawn usedBy)
    {
        usedBy.health.hediffSet.hediffs.RemoveAll(async => async.def == Caula_DefOf.ChokingOnBlood);

    }
}
