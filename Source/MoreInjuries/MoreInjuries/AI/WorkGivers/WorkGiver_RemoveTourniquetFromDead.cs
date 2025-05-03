using RimWorld;
using Verse.AI;
using Verse;
using System.Collections.Generic;
using MoreInjuries.KnownDefs;
using MoreInjuries.Caching;

namespace MoreInjuries.AI.WorkGivers;

public class WorkGiver_RemoveTourniquetFromDead : WorkGiver_Scanner
{
    private static readonly CorpseCache s_corpseCache = new();

    public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

    public override Danger MaxPathDanger(Pawn pawn) => Danger.Deadly;

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn) => s_corpseCache.GetCachedThings(pawn.Map);

    public override bool ShouldSkip(Pawn pawn, bool forced = false) => !KnownResearchProjectDefOf.BasicFirstAid.IsFinished || !s_corpseCache.HasCachedThings(pawn.Map);

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Corpse { InnerPawn: Pawn { RaceProps.Humanlike: true } deadPawn } corpse
            || corpse.IsForbidden(pawn) 
            || !deadPawn.health.hediffSet.HasHediff(KnownHediffDefOf.TourniquetApplied))
        {
            return false;
        }
        return pawn.CanReserve(corpse, ignoreOtherReservations: forced);
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        Corpse corpse = (Corpse)t;
        Job job = JobMaker.MakeJob(KnownJobDefOf.RemoveTourniquetFromDead, corpse);
        job.count = 1;
        return job;
    }

    private class CorpseCache : WeakTimedMapThingCache<Corpse>
    {
        protected override int MinCacheRefreshIntervalTicks => 1800;

        protected override IEnumerable<Corpse> GetMapThings(Map map)
        {
            List<Thing> corpses = map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse);
            foreach (Thing thing in corpses)
            {
                if (thing is Corpse { InnerPawn: Pawn { RaceProps.Humanlike: true } deadPawn } corpse 
                    && deadPawn.health.hediffSet.HasHediff(KnownHediffDefOf.TourniquetApplied))
                {
                    yield return corpse;
                }
            }
        }
    }
}