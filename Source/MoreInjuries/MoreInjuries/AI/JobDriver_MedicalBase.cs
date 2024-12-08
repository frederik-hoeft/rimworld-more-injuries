using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.AI;

public abstract class JobDriver_MedicalBase<TTarget> : JobDriver where TTarget : ThingWithComps
{
    protected const TargetIndex TARGET_INDEX = TargetIndex.A;
    protected const TargetIndex DEVICE_INDEX = TargetIndex.B;

    protected virtual int BaseTendDuration => 600;

    protected virtual float BaseExperience => 500f;

    protected Pawn Doctor => pawn;

    protected abstract TTarget GetTarget(ref readonly LocalTargetInfo targetInfo);

    protected virtual SoundDef SoundDef => SoundDefOf.Interact_Tend;

    protected virtual int CalculateTendDuration()
    {
        Pawn doctor = Doctor;
        float manipulationCapacity = Mathf.Max(doctor.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation), 0.05f);
        return (int)(1f / (doctor.GetStatValue(StatDefOf.MedicalTendSpeed) * manipulationCapacity) * BaseTendDuration);
    }

    protected abstract void FinalizeTreatment(Pawn doctor, TTarget target, Thing? thing);

    protected virtual Toil FinalizeTreatmentToil()
    {
        Toil toil = ToilMaker.MakeToil(nameof(FinalizeTreatment));
        toil.initAction = () =>
        {
            Pawn actor = toil.actor;
            Thing? thing = actor.CurJob.targetB.Thing;
            if (actor.skills is not null)
            {
                float gainFactor = thing?.def.MedicineTendXpGainFactor ?? 0.5f;
                actor.skills.Learn(SkillDefOf.Medicine, BaseExperience * gainFactor);
            }
            TTarget target = GetTarget(in actor.CurJob.targetA);
            FinalizeTreatment(actor, target, thing);
            if (actor.CurJob is null)
            {
                if (!ReferenceEquals(actor, target))
                {
                    Logger.Warning($"{nameof(FinalizeTreatment)}: Job is null after finalizing treatment. What's going on? Did our doctor die?");
                }
                else
                {
                    Logger.LogDebug($"{nameof(FinalizeTreatment)}: Job is null after finalizing treatment, but the doctor is the target. So perhaps they overdosed or died. This is probably fine.");
                }
                return;
            }
            if (thing is { Destroyed: true })
            {
                // set null to prevent double-usage
                actor.CurJob.SetTarget(DEVICE_INDEX, LocalTargetInfo.Invalid);
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
