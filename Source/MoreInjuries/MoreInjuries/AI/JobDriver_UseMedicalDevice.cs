﻿using MoreInjuries.Debug;
using MoreInjuries.Things;
using RimWorld;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.AI;

public abstract class JobDriver_UseMedicalDevice : JobDriver
{
    // NOTE: we use a ConditionalWeakTable to store non-essential while the job is scheduled for execution
    // these parameters are not saved in-between game sessions, so make sure to use sensible defaults if they are lost
    internal static readonly ConditionalWeakTable<Job, ExtendedJobParameters> s_transientJobParameters = new();
    private const int TICKS_BETWEEN_SELF_TEND_MOTES = 100;
    private const TargetIndex PATIENT_INDEX = TargetIndex.A;
    private const TargetIndex DEVICE_INDEX = TargetIndex.B;
    private const TargetIndex DEVICE_HOLDER_INDEX = TargetIndex.C;

    private static readonly List<Toil> s_tmpCollectToils = [];

    protected PathEndMode _pathEndMode;
    protected bool _usesDevice;
    protected bool _fromInventoryOnly;
    protected bool _oneShot;
    protected bool _oneShotUsed;

    protected abstract bool RequiresDevice { get; }
    
    protected abstract ThingDef DeviceDef { get; }

    protected Pawn Patient => job.targetA.Pawn;

    protected Thing? DeviceUsed => job.targetB.Thing;

    protected Pawn Doctor => pawn;

    protected virtual int BaseTendDuration => 600;

    protected abstract void ApplyDevice(Pawn doctor, Pawn patient, Thing? device);

    protected abstract bool IsTreatable(Hediff hediff);

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref _usesDevice, nameof(_usesDevice));
        Scribe_Values.Look(ref _pathEndMode, nameof(_pathEndMode));
        Scribe_Values.Look(ref _fromInventoryOnly, nameof(_fromInventoryOnly));
        Scribe_Values.Look(ref _oneShot, nameof(_oneShot));
        Scribe_Values.Look(ref _oneShotUsed, nameof(_oneShotUsed));
    }

    public override void Notify_Starting()
    {
        base.Notify_Starting();
        if (s_transientJobParameters.TryGetValue(job, out ExtendedJobParameters? parameters))
        {
            _fromInventoryOnly = parameters.FromInventoryOnly;
            _oneShot = parameters.OneShot;
            s_transientJobParameters.Remove(job);
        }
        _oneShotUsed = false;
        _usesDevice = DeviceUsed is not null;
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
        if (RequiresDevice && DeviceUsed is null)
        {
            // we need a splint to do the job
            return false;
        }
        if (DeviceUsed is not null)
        {
            // we allow up to 10 pawns to concurrently reserve devices from the same stack (taken from JobDriver_TendPatient)
            int availableDevices = Doctor.Map.reservationManager.CanReserveStack(Doctor, DeviceUsed, maxPawns: MedicalDeviceHelper.MAX_MEDICAL_DEVICE_RESERVATIONS);
            int requiredDevices = 1;
            if (!_oneShot)
            {
                requiredDevices = MedicalDeviceHelper.GetMedicalDeviceCountToFullyHeal(Patient, IsTreatable);
            }
            // attempt to reserve a splint
            if (availableDevices >= 1 && Doctor.Reserve(DeviceUsed, job, MedicalDeviceHelper.MAX_MEDICAL_DEVICE_RESERVATIONS, 
                Mathf.Min(availableDevices, requiredDevices),
                errorOnFailed: errorOnFailed))
            {
                return true;
            }
            return false;
        }
        return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        Pawn doctor = Doctor;
        Pawn patient = Patient;
        this.FailOnDespawnedNullOrForbidden(PATIENT_INDEX);
        this.FailOn(() =>
        {
            // we can't apply a devices if the patient is set to no medical care
            if (doctor.Faction == Faction.OfPlayer && patient.playerSettings?.medCare is MedicalCareCategory.NoCare
                // we can't apply a device if the doctor wants to tend himself but is set to no self-tend
                || doctor == patient && doctor.Faction == Faction.OfPlayer && doctor.playerSettings?.selfTend is false)
            {
                return true;
            }
            return false;
        });
        this.FailOnAggroMentalState(PATIENT_INDEX);

        AddEndCondition(() =>
        {
            // if the job is a one-shot and it has been used
            if (_oneShot && _oneShotUsed)
            {
                return JobCondition.Succeeded;
            }
            // continue tending the patient if they have the target hediff ...
            if (patient.health.hediffSet.hediffs.Any(hediff => IsTreatable(hediff))
                // ... and the doctor is able to tend the patient ...
                && (doctor.Faction == Faction.OfPlayer && HealthAIUtility.ShouldEverReceiveMedicalCareFromPlayer(Patient)
                // or if the doctor is forced to tend the patient or belongs to a different faction (AI controlled)
                || (job.playerForced || doctor.Faction != Faction.OfPlayer)))
            {
                return JobCondition.Ongoing;
            }
            Logger.LogDebug($"Finished applying {DeviceDef} to {patient.Name}");
            return JobCondition.Succeeded;
        });
        Toil? reserveDevices = null;
        Toil gotoPatientToil = Toils_Goto.Goto(PATIENT_INDEX, _pathEndMode);
        gotoPatientToil.AddFinishAction(() => patient.jobs.posture = PawnPosture.LayingOnGroundFaceUp);
        if (_usesDevice)
        {
            List<Toil> collectDeviceToils = CollectDevicesToils(doctor, patient, job, gotoPatientToil, out reserveDevices);
            foreach (Toil toil in collectDeviceToils)
            {
                yield return toil;
            }
        }
        yield return gotoPatientToil;
        float manipulationCapacity = Mathf.Max(doctor.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation), 0.05f);
        int ticks = (int)(1f / (doctor.GetStatValue(StatDefOf.MedicalTendSpeed) * manipulationCapacity) * BaseTendDuration);
        Toil waitToil;
        if (doctor == patient)
        {
            waitToil = Toils_General.Wait(ticks: ticks);
        }
        else
        {
            waitToil = Toils_General.WaitWith_NewTemp(PATIENT_INDEX, ticks, maintainPosture: true, face: PATIENT_INDEX, pathEndMode: _pathEndMode);
            waitToil.AddFinishAction(() =>
            {
                if (patient != doctor && patient?.CurJob?.def is JobDef jobDef && (jobDef == JobDefOf.Wait || jobDef == JobDefOf.Wait_MaintainPosture))
                {
                    patient.jobs.EndCurrentJob(JobCondition.InterruptForced, startNewJob: true, canReturnToPool: true);
                }
            });
        }
        waitToil.WithProgressBarToilDelay(PATIENT_INDEX).PlaySustainerOrSound(SoundDefOf.Interact_Tend);
        waitToil.activeSkill = () => SkillDefOf.Medicine;
        waitToil.handlingFacing = true;
        waitToil.tickAction = () =>
        {
            if (doctor == patient && doctor.Faction != Faction.OfPlayer && doctor.IsHashIntervalTick(TICKS_BETWEEN_SELF_TEND_MOTES) && !doctor.Position.Fogged(doctor.Map))
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
        yield return Toils_Jump.JumpIf(waitToil, () => !_usesDevice || DeviceUsed is not null && doctor.inventory.Contains(DeviceUsed));
        yield return Toils_MedicalDevice.PickupDevice(DEVICE_INDEX, patient, IsTreatable).FailOnDestroyedOrNull(DEVICE_INDEX);
        yield return waitToil;
        yield return Toils_MedicalDevice.FinalizeApplyDevice(patient, ApplyDeviceCore);
        if (_usesDevice)
        {
            DebugAssert.NotNull(reserveDevices, $"{nameof(reserveDevices)} cannot be null when using a device");
            yield return FindMoreDevicesToil(doctor, patient, DEVICE_INDEX, job, reserveDevices!);
        }
        yield return Toils_Jump.Jump(gotoPatientToil);
    }

    private void ApplyDeviceCore(Pawn doctor, Pawn patient, Thing? device)
    {
        ApplyDevice(doctor, patient, device);
        _oneShotUsed = true;
    }

    public override void Notify_DamageTaken(DamageInfo dinfo)
    {
        base.Notify_DamageTaken(dinfo);
        if (!dinfo.Def.ExternalViolenceFor(Doctor) || pawn.Faction == Faction.OfPlayer || Doctor != Patient)
        {
            return;
        }
        pawn.jobs.CheckForJobOverride();
    }

    private List<Toil> CollectDevicesToils(Pawn doctor, Pawn patient, Job job, Toil gotoToil, out Toil reserveDevices)
    {
        s_tmpCollectToils.Clear();
        Thing deviceUsed = job.targetB.Thing;
        Pawn_InventoryTracker? deviceHolderInventory = deviceUsed.ParentHolder as Pawn_InventoryTracker;
        Pawn otherPawnDeviceHolder = job.targetC.Pawn;

        reserveDevices = Toils_MedicalDevice.ReserveDevice(DEVICE_INDEX, patient, IsTreatable)
            .FailOnDespawnedNullOrForbidden(DEVICE_INDEX);
        s_tmpCollectToils.Add(Toils_Jump.JumpIf(gotoToil, () => deviceUsed is not null && doctor.inventory.Contains(deviceUsed)));
        Toil jumpIfCarriedByOther = Toils_Goto.GotoThing(DEVICE_HOLDER_INDEX, PathEndMode.Touch)
            .FailOn(() => otherPawnDeviceHolder != deviceHolderInventory?.pawn
                || otherPawnDeviceHolder.IsForbidden(doctor));
        s_tmpCollectToils.Add(Toils_Haul.CheckItemCarriedByOtherPawn(deviceUsed, DEVICE_HOLDER_INDEX, jumpIfCarriedByOther));
        s_tmpCollectToils.Add(reserveDevices);
        s_tmpCollectToils.Add(Toils_Goto.GotoThing(DEVICE_INDEX, PathEndMode.ClosestTouch)
            .FailOnDespawnedNullOrForbidden(DEVICE_INDEX));
        s_tmpCollectToils.Add(Toils_MedicalDevice.PickupDevice(DEVICE_INDEX, patient, IsTreatable)
            .FailOnDestroyedOrNull(DEVICE_INDEX));
        s_tmpCollectToils.Add(Toils_Haul.CheckForGetOpportunityDuplicate(reserveDevices, DEVICE_INDEX, TargetIndex.None, takeFromValidStorage: true));
        s_tmpCollectToils.Add(Toils_Jump.Jump(gotoToil));
        s_tmpCollectToils.Add(jumpIfCarriedByOther);
        s_tmpCollectToils.Add(Toils_General.Wait(25).WithProgressBarToilDelay(DEVICE_HOLDER_INDEX));
        s_tmpCollectToils.Add(Toils_Haul.TakeFromOtherInventory(
            deviceUsed,
            doctor.inventory.innerContainer,
            deviceHolderInventory?.innerContainer,
            MedicalDeviceHelper.GetMedicalDeviceCountToFullyHeal(patient, IsTreatable),
            DEVICE_INDEX));
        return s_tmpCollectToils;
    }

    private Toil FindMoreDevicesToil(Pawn doctor, Pawn patient, TargetIndex deviceIndex, Job job, Toil reserveDevice)
    {
        Toil toil = ToilMaker.MakeToil(nameof(FindMoreDevicesToil));
        toil.initAction = () =>
        {
            if (!job.GetTarget(deviceIndex).Thing.DestroyedOrNull())
            {
                return;
            }
            Thing? device = MedicalDeviceHelper.FindMedicalDevice(doctor, patient, DeviceDef, IsTreatable, _fromInventoryOnly);
            if (device is not null)
            {
                job.SetTarget(deviceIndex, device);
                doctor.jobs.curDriver.JumpToToil(reserveDevice);
            }
        };
        return toil;
    }

    public record ExtendedJobParameters(bool FromInventoryOnly = false, bool OneShot = false);
}