using MoreInjuries.Things;
using RimWorld;
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

    public static Toil FinalizeApplyDevice(Pawn patient, ApplyDeviceAction applyDeviceAction)
    {
        Toil toil = ToilMaker.MakeToil(nameof(FinalizeApplyDevice));
        toil.initAction = () =>
        {
            Pawn actor = toil.actor;
            Thing? thing = actor.CurJob.targetB.Thing;
            if (actor.skills is not null)
            {
                float baseExperience = 500f;
                float gainFactor = thing?.def.MedicineTendXpGainFactor ?? 0.5f;
                actor.skills.Learn(SkillDefOf.Medicine, baseExperience * gainFactor);
            }
            applyDeviceAction.Invoke(actor, patient, thing);
            if (actor.CurJob is null)
            {
                if (actor != patient)
                {
                    Logger.Warning($"{nameof(FinalizeApplyDevice)}: Job is null after applying device. What's going on? Did our doctor die?");
                }
                else
                {
                    Logger.LogDebug($"{nameof(FinalizeApplyDevice)}: Job is null after applying device, but the doctor is the patient. So they probably overdosed or died. This is fine.");
                }
                return;
            }
            if (thing is { Destroyed: true })
            {
                // set null to prevent double-usage
                actor.CurJob.SetTarget(TargetIndex.B, LocalTargetInfo.Invalid);
            }

            if (!actor.CurJob.endAfterTendedOnce)
            {
                return;
            }

            actor.jobs.EndCurrentJob(JobCondition.Succeeded);
        };
        toil.defaultCompleteMode = ToilCompleteMode.Instant;
        return toil;
    }
}