﻿using RimWorld;
using System.Collections.Generic;
using Verse.AI;
using Verse;

namespace MoreInjuries.AI.WorkGivers;

public abstract class WorkGiver_MoreInjuriesTreatmentBase : WorkGiver_Scanner
{
    protected virtual bool CanTreat(Hediff hediff) => 
        throw new NotSupportedException($"WorkGiver of type {GetType().Name} does not support CanTreat(Hediff) method. Did you forget to override {nameof(HasJobOnThing)}? This is a bug.");

    protected abstract Job CreateJob(Pawn doctor, Pawn patient);

    public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

    public override Danger MaxPathDanger(Pawn pawn) => Danger.Deadly;

    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Pawn);

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn) => pawn.Map.mapPawns.SpawnedHumanlikesWithAnyHediff;

    protected virtual bool IsValidPatient(Pawn doctor, Thing thing, out Pawn patient)
    {
        if (thing is not Pawn)
        {
            patient = null!;
            return false;
        }
        patient = (Pawn)thing;
        return doctor != patient
            && GoodLayingStatusForTend(patient, doctor)
            && !patient.IsForbidden(doctor)
            && (!patient.IsMutant || patient.mutant.Def.entitledToMedicalCare)
            && !patient.InAggroMentalState;
    }

    public override bool HasJobOnThing(Pawn pawn, Thing thing, bool forced = false)
    {
        if (IsValidPatient(pawn, thing, out Pawn patient) && CanTreat(pawn, patient))
        {
            return pawn.CanReserve(patient, ignoreOtherReservations: forced);
        }
        return false;
    }

    protected virtual bool CanTreat(Pawn doctor, Pawn patient) => patient.health.hediffSet.hediffs.Any(CanTreat);

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

    protected Job GetDummyDefaultJob(Pawn doctor)
    {
        Logger.Warning($"{GetType().Name} used fallback to empty dummy job");
        return JobMaker.MakeJob(JobDefOf.Goto, doctor);
    }
}
