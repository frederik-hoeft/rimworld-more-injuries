using MoreInjuries.Things;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.AI;

internal static class Toils_MedicalDevice
{
    public static Toil ReserveDevice(TargetIndex targetIndex, Pawn patient, Func<Pawn, int> getDeviceCountToFullyHeal)
    {
        Toil toil = ToilMaker.MakeToil(nameof(ReserveDevice));
        toil.initAction = () =>
        {
            Pawn actor = toil.actor;
            Job curJob = actor.jobs.curJob;
            Thing thing = curJob.GetTarget(targetIndex).Thing;
            int availableDevices = actor.Map.reservationManager.CanReserveStack(actor, thing, MedicalDeviceHelper.MAX_MEDICAL_DEVICE_RESERVATIONS);
            if (availableDevices > 0 && actor.Reserve(thing, curJob, MedicalDeviceHelper.MAX_MEDICAL_DEVICE_RESERVATIONS, Mathf.Min(availableDevices, getDeviceCountToFullyHeal(patient))))
            {
                return;
            }

            toil.actor.jobs.EndCurrentJob(JobCondition.Incompletable);
        };
        toil.defaultCompleteMode = ToilCompleteMode.Instant;
        toil.atomicWithPrevious = true;
        return toil;
    }

    public static Toil PickupDevice(TargetIndex targetIndex, Pawn patient, Func<Pawn, int> getDeviceCountToFullyHeal)
    {
        Toil toil = ToilMaker.MakeToil(nameof(PickupDevice));
        toil.initAction = () =>
        {
            Pawn actor = toil.actor;
            Job curJob = actor.jobs.curJob;
            Thing thing = curJob.GetTarget(targetIndex).Thing;
            int countToFullyHeal = getDeviceCountToFullyHeal(patient);
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
}