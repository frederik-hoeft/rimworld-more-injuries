using RimWorld;
using Verse.AI;
using Verse;
using System.Collections.Generic;
using MoreInjuries.KnownDefs;

namespace MoreInjuries.AI.WorkGivers;

public class WorkGiver_RemoveTourniquetFromDead : WorkGiver_Scanner
{
    public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

    public override Danger MaxPathDanger(Pawn pawn) => Danger.Deadly;

    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Corpse);

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn) => pawn.Map.spawnedThings;

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Corpse corpse
            || corpse.IsForbidden(pawn) 
            || !corpse.InnerPawn.health.hediffSet.HasHediff(KnownHediffDefOf.TourniquetApplied))
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
}
