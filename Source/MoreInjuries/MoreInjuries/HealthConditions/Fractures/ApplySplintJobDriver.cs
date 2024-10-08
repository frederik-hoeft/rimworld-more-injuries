using System.Collections.Generic;
using MoreInjuries.KnownDefs;
using MoreInjuries.Things;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.Fractures;

// tend all fractures on a patient
public class ApplySplintJobDriver : JobDriver
{
    private PathEndMode _pathEndMode;

    protected Pawn Patient => job.targetA.Pawn;

    protected Thing SplintUsed => job.targetB.Thing;

    protected Pawn Doctor => pawn;

    private static readonly List<Toil> s_tmpCollectToils = [];

    public override void Notify_Starting()
    {
        base.Notify_Starting();
        if (Patient == Doctor)
        {
            _pathEndMode = PathEndMode.OnCell;
        }
        else if (Patient.InBed())
        {
            _pathEndMode = PathEndMode.InteractionCell;
        }
        else
        {
            _pathEndMode = PathEndMode.Touch;
        }
    }

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        if (Patient != Doctor && !Doctor.Reserve(Patient, job, errorOnFailed: errorOnFailed))
        {
            // we couldn't reserve the patient, so we can't do the job
            return false;
        }
        if (SplintUsed is null)
        {
            // we need a splint to do the job
            return false;
        }
        // we allow up to 10 pawns to concurrently reserve splints from the same stack (taken from JobDriver_TendPatient)
        int availableSplints = Doctor.Map.reservationManager.CanReserveStack(Doctor, SplintUsed, maxPawns: MedicalDeviceHelper.MAX_MEDICAL_DEVICE_RESERVATIONS);
        // attempt to reserve a splint
        return availableSplints >= 1 && Doctor.Reserve(
            SplintUsed, 
            job, 
            maxPawns: MedicalDeviceHelper.MAX_MEDICAL_DEVICE_RESERVATIONS, 
            stackCount: Mathf.Min(availableSplints, MedicalDeviceHelper.GetMedicalDeviceCountToFullyHeal(Patient, KnownHediffDefOf.Fracture)), 
            errorOnFailed: errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        Pawn doctor = Doctor;
        Pawn patient = Patient;
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOn(() =>
        {
            // TODO: double-check this logic (doctor should be able to resupply splints if they are null)
            // we can't apply a splint if there is no splint
            if (SplintUsed is null
                // we can't apply a splint if the patient is set to no medical care
                || doctor.Faction == Faction.OfPlayer && patient.playerSettings?.medCare is MedicalCareCategory.NoCare
                // we can't apply a splint if the doctor wants to tend himself but is set to no self-tend
                || doctor == patient && doctor.Faction == Faction.OfPlayer && doctor.playerSettings?.selfTend is false)
            {
                Log.Warning($"Failed to apply splint to {patient.Name}");
                return true;
            }
            return false;
        });
        this.FailOnAggroMentalState(TargetIndex.A);

        AddEndCondition(() =>
        {
            // continue tending the patient if they have fractures ...
            if (patient.health.hediffSet.HasHediff(KnownHediffDefOf.Fracture)
                // ... and the doctor is able to tend the patient ...
                && (doctor.Faction == Faction.OfPlayer && HealthAIUtility.ShouldEverReceiveMedicalCareFromPlayer(Patient)
                // or if the doctor is forced to tend the patient or belongs to a different faction (AI controlled)
                || (job.playerForced || doctor.Faction != Faction.OfPlayer)))
            {
                return JobCondition.Ongoing;
            }
            Logger.LogDebug($"Finished applying splint to {patient.Name}");
            return JobCondition.Succeeded;
        });
        Toil gotoPatientToil = Toils_Goto.Goto(TargetIndex.A, _pathEndMode);
        gotoPatientToil.AddFinishAction(() => patient.jobs.posture = PawnPosture.LayingOnGroundFaceUp);
        List<Toil> collectSplintToils = CollectSplintsToils(doctor, patient, job, gotoPatientToil, out Toil? reserveSplints);
        foreach (Toil toil in collectSplintToils)
        {
            yield return toil;
        }
        yield return gotoPatientToil;
        float manipulationCapacity = Mathf.Max(doctor.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation), 0.05f);
        int ticks = (int)(1f / (doctor.GetStatValue(StatDefOf.MedicalTendSpeed) * manipulationCapacity) * 500.0f);
        Toil waitToil;
        if (doctor == patient)
        {
            waitToil = Toils_General.Wait(ticks: ticks);
        }
        else
        {
            waitToil = Toils_General.WaitWith_NewTemp(TargetIndex.A, ticks, maintainPosture: true, face: TargetIndex.A, pathEndMode: _pathEndMode);
            waitToil.AddFinishAction(() =>
            {
                if (patient != doctor && patient?.CurJob?.def is JobDef jobDef && (jobDef == JobDefOf.Wait || jobDef == JobDefOf.Wait_MaintainPosture))
                {
                    patient.jobs.EndCurrentJob(JobCondition.InterruptForced, startNewJob: true, canReturnToPool: true);
                }
            });
        }
        waitToil.WithProgressBarToilDelay(TargetIndex.A).PlaySustainerOrSound(SoundDefOf.Interact_Tend);
        waitToil.activeSkill = () => SkillDefOf.Medicine;
        waitToil.handlingFacing = true;
        waitToil.tickAction = () =>
        {
            if (doctor == patient && doctor.Faction != Faction.OfPlayer && doctor.IsHashIntervalTick(100) && !doctor.Position.Fogged(doctor.Map))
            {
                // AI self-heal animation
                FleckMaker.ThrowMetaIcon(doctor.Position, doctor.Map, FleckDefOf.HealingCross, velocitySpeed: 0.42f);
            }
            if (doctor != patient)
            {
                doctor.rotationTracker.FaceTarget(patient);
            }
        };
        waitToil.FailOn(() =>
        {
            if (doctor == patient)
            {
                return false;
            }
            return !ReachabilityImmediate.CanReachImmediate(doctor, patient.SpawnedParentOrMe, _pathEndMode);
        });
        // TODO: was inverted in the original code, but it seems like it should be the other way around, verify!
        yield return Toils_Jump.JumpIf(waitToil, () => doctor.inventory.Contains(SplintUsed));
        yield return Toils_MedicalDevice.PickupDevice(TargetIndex.B, patient, KnownHediffDefOf.Fracture).FailOnDestroyedOrNull(TargetIndex.B);
        yield return waitToil;
        yield return Toils_MedicalDevice.FinalizeApplyDevice(patient, ApplySplint);
        yield return FindMoreSplintsToil_NewTemp(doctor, patient, TargetIndex.B, job, reserveSplints);
        yield return Toils_Jump.Jump(gotoPatientToil);
    }

    private static void ApplySplint(Pawn doctor, Pawn patient, Thing? splint)
    {
        Hediff? fracture = patient.health.hediffSet.hediffs.Find(hediff => hediff.def == KnownHediffDefOf.Fracture);
        if (fracture is { Part: BodyPartRecord part })
        {
            Hediff healingFracture = HediffMaker.MakeHediff(KnownHediffDefOf.FractureHealing, patient, part);
            healingFracture.Severity = 1f;
            patient.health.AddHediff(healingFracture);
            patient.health.RemoveHediff(fracture);
            splint?.DecreaseStack();
        }
    }

    public static List<Toil> CollectSplintsToils(Pawn doctor, Pawn patient, Job job, Toil gotoToil, out Toil reserveSplints)
    {
        s_tmpCollectToils.Clear();
        Thing splintUsed = job.targetB.Thing;
        Pawn_InventoryTracker? medicineHolderInventory = splintUsed.ParentHolder as Pawn_InventoryTracker;
        Pawn otherPawnMedicineHolder = job.targetC.Pawn;

        reserveSplints = Toils_MedicalDevice.ReserveDevice(TargetIndex.B, patient, KnownHediffDefOf.Fracture)
            .FailOnDespawnedNullOrForbidden(TargetIndex.B);
        s_tmpCollectToils.Add(Toils_Jump.JumpIf(gotoToil, () => doctor.inventory.Contains(splintUsed)));
        Toil jumpIfCarriedByOther = Toils_Goto.GotoThing(TargetIndex.C, PathEndMode.Touch)
            .FailOn(() => 
                otherPawnMedicineHolder != medicineHolderInventory?.pawn 
                || otherPawnMedicineHolder.IsForbidden(doctor));
        s_tmpCollectToils.Add(Toils_Haul.CheckItemCarriedByOtherPawn(splintUsed, TargetIndex.C, jumpIfCarriedByOther));
        s_tmpCollectToils.Add(reserveSplints);
        s_tmpCollectToils.Add(Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch)
            .FailOnDespawnedNullOrForbidden(TargetIndex.B));
        s_tmpCollectToils.Add(Toils_MedicalDevice.PickupDevice(TargetIndex.B, patient, KnownHediffDefOf.Fracture)
            .FailOnDestroyedOrNull(TargetIndex.B));
        s_tmpCollectToils.Add(Toils_Haul.CheckForGetOpportunityDuplicate(reserveSplints, TargetIndex.B, TargetIndex.None, takeFromValidStorage: true));
        s_tmpCollectToils.Add(Toils_Jump.Jump(gotoToil));
        s_tmpCollectToils.Add(jumpIfCarriedByOther);
        s_tmpCollectToils.Add(Toils_General.Wait(25).WithProgressBarToilDelay(TargetIndex.C));
        s_tmpCollectToils.Add(Toils_Haul.TakeFromOtherInventory(
            splintUsed, 
            doctor.inventory.innerContainer, 
            medicineHolderInventory?.innerContainer, 
            MedicalDeviceHelper.GetMedicalDeviceCountToFullyHeal(patient, KnownHediffDefOf.Fracture), 
            TargetIndex.B));
        return s_tmpCollectToils;
    }

    public static Toil FindMoreSplintsToil_NewTemp(Pawn doctor, Pawn patient, TargetIndex splintIndex, Job job, Toil reserveSplint)
    {
        Toil toil = ToilMaker.MakeToil(nameof(FindMoreSplintsToil_NewTemp));
        toil.initAction = () =>
        {
            if (!job.GetTarget(splintIndex).Thing.DestroyedOrNull())
            {
                return;
            }
            Thing? splint = MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.Splint, KnownHediffDefOf.Fracture);
            if (splint is not null)
            {
                job.SetTarget(splintIndex, splint);
                doctor.jobs.curDriver.JumpToToil(reserveSplint);
            }
        };
        return toil;
    }
}
