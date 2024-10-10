using MoreInjuries.Things;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.AI;

internal static class Toils_MedicalDevice
{
    public static Toil ReserveDevice(TargetIndex targetIndex, Pawn patient, Predicate<Hediff> isTreatableWithDevice)
    {
        Toil toil = ToilMaker.MakeToil(nameof(ReserveDevice));
        toil.initAction = () =>
        {
            Pawn actor = toil.actor;
            Job curJob = actor.jobs.curJob;
            Thing thing = curJob.GetTarget(targetIndex).Thing;
            int availableDevices = actor.Map.reservationManager.CanReserveStack(actor, thing, MedicalDeviceHelper.MAX_MEDICAL_DEVICE_RESERVATIONS);
            if (availableDevices > 0 && actor.Reserve(thing, curJob, MedicalDeviceHelper.MAX_MEDICAL_DEVICE_RESERVATIONS, Mathf.Min(availableDevices, MedicalDeviceHelper.GetMedicalDeviceCountToFullyHeal(patient, isTreatableWithDevice))))
            {
                return;
            }

            toil.actor.jobs.EndCurrentJob(JobCondition.Incompletable);
        };
        toil.defaultCompleteMode = ToilCompleteMode.Instant;
        toil.atomicWithPrevious = true;
        return toil;
    }

    public static Toil PickupDevice(TargetIndex targetIndex, Pawn patient, Predicate<Hediff> isTreatableWithDevice)
    {
        Toil toil = ToilMaker.MakeToil(nameof(PickupDevice));
        toil.initAction = () =>
        {
            Pawn actor = toil.actor;
            Job curJob = actor.jobs.curJob;
            Thing thing = curJob.GetTarget(targetIndex).Thing;
            int countToFullyHeal = MedicalDeviceHelper.GetMedicalDeviceCountToFullyHeal(patient, isTreatableWithDevice);
            if (actor.carryTracker.CarriedThing is not null)
            {
                countToFullyHeal -= actor.carryTracker.CarriedThing.stackCount;
            }

            int count = Mathf.Min(actor.Map.reservationManager.CanReserveStack(actor, thing, MedicalDeviceHelper.MAX_MEDICAL_DEVICE_RESERVATIONS), countToFullyHeal);
            if (count > 0)
            {
                actor.carryTracker.TryStartCarry(thing, count);
            }

            curJob.count = countToFullyHeal - count;
            if (thing.Spawned)
            {
                toil.actor.Map.reservationManager.Release(thing, actor, curJob);
            }

            curJob.SetTarget(targetIndex, actor.carryTracker.CarriedThing);
        };
        toil.defaultCompleteMode = ToilCompleteMode.Instant;
        return toil;
    }

    public static Toil FinalizeApplyDevice(Pawn patient, ApplyDeviceAction applyDeviceAction)
    {
        Toil toil = ToilMaker.MakeToil(nameof(FinalizeApplyDevice));
        toil.initAction = () =>
        {
            Pawn actor = toil.actor;
            Thing thing = actor.CurJob.targetB.Thing;
            if (actor.skills is not null)
            {
                float baseExperience = 500f;
                float gainFactor = thing?.def.MedicineTendXpGainFactor ?? 0.5f;
                actor.skills.Learn(SkillDefOf.Medicine, baseExperience * gainFactor);
            }
            applyDeviceAction.Invoke(actor, patient, thing);
            if (thing != null && thing.Destroyed)
            {
                actor.CurJob.SetTarget(TargetIndex.B, LocalTargetInfo.Invalid);
            }

            if (!toil.actor.CurJob.endAfterTendedOnce)
            {
                return;
            }

            actor.jobs.EndCurrentJob(JobCondition.Succeeded);
        };
        toil.defaultCompleteMode = ToilCompleteMode.Instant;
        return toil;
    }
}