using RimWorld;
using System.Collections.Generic;
using Verse.AI;
using Verse;

namespace MoreInjuries.AI.WorkGivers;

public abstract class WorkGiver_MoreInjuriesTreatmentBase : WorkGiver_Scanner
{
    protected abstract bool CanTreat(Hediff hediff);

    protected abstract Job CreateJob(Pawn doctor, Pawn patient);

    public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

    public override Danger MaxPathDanger(Pawn pawn) => Danger.Deadly;

    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Pawn);

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn) => pawn.Map.mapPawns.SpawnedHumanlikesWithAnyHediff;

    public override bool HasJobOnThing(Pawn pawn, Thing thing, bool forced = false)
    {
        if (thing is Pawn patient
            && pawn != patient
            && GoodLayingStatusForTend(patient, pawn)
            && !patient.IsForbidden(pawn)
            && (!patient.IsMutant || patient.mutant.Def.entitledToMedicalCare)
            && !patient.InAggroMentalState)
        {
            foreach (Hediff hediff in patient.health.hediffSet.hediffs)
            {
                if (CanTreat(hediff))
                {
                    return pawn.CanReserve((LocalTargetInfo)patient, ignoreOtherReservations: forced);
                }
            }
        }
        return false;
    }

    public static bool GoodLayingStatusForTend(Pawn patient, Pawn doctor) => patient != doctor && patient.InBed();

    public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
    {
        if (thing is not Pawn patient)
        {
            Logger.Error($"{nameof(WorkGiver_MoreInjuriesTreatmentBase)} expected {thing} to be a Pawn, but it was somethig else instead :C ... returning dummy job");
            return GetDummyDefaultJob(pawn);
        }
        return CreateJob(pawn, patient);
    }

    private static Job GetDummyDefaultJob(Pawn doctor) => JobMaker.MakeJob(JobDefOf.Goto, doctor);
}
