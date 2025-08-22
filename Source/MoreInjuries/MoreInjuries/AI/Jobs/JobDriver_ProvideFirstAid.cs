using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Extensions;
using MoreInjuries.HealthConditions.CardiacArrest;
using MoreInjuries.HealthConditions.Choking;
using MoreInjuries.HealthConditions.Drugs.Epinephrine;
using MoreInjuries.HealthConditions.HeavyBleeding;
using MoreInjuries.HealthConditions.HeavyBleeding.Bandages;
using MoreInjuries.HealthConditions.HeavyBleeding.HemostaticAgents;
using MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;
using MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;
using MoreInjuries.Things;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.AI.Jobs;

public sealed class JobDriver_ProvideFirstAid : JobDriver
{
    private const TargetIndex PATIENT_INDEX = TargetIndex.A;

    private Pawn Doctor => pawn;

    private Pawn Patient => job.targetA.Pawn;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        if (Patient != Doctor && !Doctor.Reserve(Patient, job, errorOnFailed: errorOnFailed))
        {
            // we couldn't reserve the patient, so we can't do the job
            return false;
        }
        return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(PATIENT_INDEX);
        this.FailOnAggroMentalState(PATIENT_INDEX);

        Toil toil = ToilMaker.MakeToil(nameof(TryScanForInjuries));
        toil.initAction = () =>
        {
            if (!TryScanForInjuries())
            {
                EndJobWith(JobCondition.Succeeded);
            }
        };
        toil.defaultCompleteMode = ToilCompleteMode.Instant;
        return [toil];
    }

    private bool TryScanForInjuries()
    {
        Pawn doctor = Doctor;
        Pawn patient = Patient;
        float bleedRateTotal = patient.health.hediffSet.BleedRateTotal;

        Job job;
        // check if we can/need to apply a tourniquet
        if (bleedRateTotal > Mathf.Epsilon)
        {
            using BleedRateByLimbEnumerable bleedRateCache = BleedRateByLimbEnumerable.EvaluateLimbs(patient);
            bool pawnKnowsWhatTheyreDoing = JobDriver_TourniquetBase.PawnKnowsWhatTheyreDoing(doctor);
            foreach ((BodyPartRecord bodyPart, float bleedRate) in bleedRateCache.OrderByDescending(static kvp => kvp.Value))
            {
                // only apply a tourniquet if the bleed rate is high enough and we have a tourniquet in our inventory
                if (bleedRate > MoreInjuriesMod.Settings.MinBleedRateForAutoTourniquet 
                    && MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.Tourniquet, fromInventoryOnly: true) is Thing tourniquet
                    // for medically and intellectually challenged doctors, there's a small chance of certain accidents...
                    && (bodyPart.def != KnownBodyPartDefOf.Neck || !pawnKnowsWhatTheyreDoing && RandomX.Shared.NextDouble() < 0.2d))
                {
                    job = JobDriver_UseTourniquet.GetDispatcher(doctor, patient, tourniquet, bodyPart).CreateJob();
                    return StartJobAndScheduleScan(doctor, patient, job);
                }
            }
        }
        // first stop the bleeding (inventory first)
        if (patient.health.hediffSet.hediffs.Any(JobDriver_HemostasisBase.JobCanTreat))
        {
            if (MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.HemostaticAgent, JobDriver_HemostasisBase.JobCanTreat, fromInventoryOnly: true) is Thing inventoryHemostaticAgent)
            {
                job = JobDriver_UseHemostaticAgent.GetDispatcher(doctor, patient, inventoryHemostaticAgent, fromInventoryOnly: true).CreateJob();
                return StartJobAndScheduleScan(doctor, patient, job);
            }
            if (MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.Bandage, JobDriver_HemostasisBase.JobCanTreat, fromInventoryOnly: true) is Thing inventoryBandage)
            {
                job = JobDriver_UseBandage.GetDispatcher(doctor, patient, inventoryBandage, fromInventoryOnly: true).CreateJob();
                return StartJobAndScheduleScan(doctor, patient, job);
            }
            if (MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.HemostaticAgent, JobDriver_HemostasisBase.JobCanTreat) is Thing hemostaticAgent)
            {
                job = JobDriver_UseHemostaticAgent.GetDispatcher(doctor, patient, hemostaticAgent, fromInventoryOnly: false).CreateJob();
                return StartJobAndScheduleScan(doctor, patient, job);
            }
            if (MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.Bandage, JobDriver_HemostasisBase.JobCanTreat) is Thing bandage)
            {
                job = JobDriver_UseBandage.GetDispatcher(doctor, patient, bandage, fromInventoryOnly: false).CreateJob();
                return StartJobAndScheduleScan(doctor, patient, job);
            }
        }
        // next, see if we need to defibrillate
        if (patient.health.hediffSet.hediffs.Any(JobDriver_UseDefibrillator.JobCanTreat))
        {
            // check if we can administer epinephrine to make the cardiac arrest shockable / boost CPR effectiveness
            if ((!patient.health.hediffSet.TryGetHediff(KnownHediffDefOf.AdrenalineRush, out Hediff? epinephrine) || epinephrine.Severity < JobDriver_UseEpinephrine.SAFETY_THRESHOLD)
                && MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.Epinephrine, fromInventoryOnly: true) is Thing epinephrineInjector)
            {
                job = JobDriver_UseEpinephrine.GetDispatcher(doctor, patient, epinephrineInjector).CreateJob();
                return StartJobAndScheduleScan(doctor, patient, job);
            }
            if (MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.Defibrillator, JobDriver_UseDefibrillator.JobCanTreat, fromInventoryOnly: true) is Thing defibrillator)
            {
                job = JobDriver_UseDefibrillator.GetDispatcher(doctor, patient, defibrillator).CreateJob();
                return StartJobAndScheduleScan(doctor, patient, job);
            }
            job = JobDriver_PerformCpr.GetDispatcher(doctor, patient).CreateJob();
            return StartJobAndScheduleScan(doctor, patient, job);
        }
        // is the patient choking?
        if (patient.health.hediffSet.hediffs.Any(static hediff => Array.IndexOf(JobDriver_UseSuctionDevice.TargetHediffDefs, hediff.def) != -1))
        {
            if (MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.SuctionDevice, JobDriver_UseSuctionDevice.TargetHediffDefs, fromInventoryOnly: true) is Thing suctionDevice)
            {
                job = JobDriver_UseSuctionDevice.GetDispatcher(doctor, patient, suctionDevice).CreateJob();
                return StartJobAndScheduleScan(doctor, patient, job);
            }
            job = JobDriver_PerformCpr.GetDispatcher(doctor, patient).CreateJob();
            return StartJobAndScheduleScan(doctor, patient, job);
        }
        // do we need to perform CPR?
        if (patient.health.hediffSet.hediffs.Any(static hediff => Array.IndexOf(JobDriver_PerformCpr.TargetHediffDefs, hediff.def) != -1))
        {
            job = JobDriver_PerformCpr.GetDispatcher(doctor, patient).CreateJob();
            return StartJobAndScheduleScan(doctor, patient, job);
        }
        // is an immedite transfusion required?
        // prefer cheaper saline bags over blood bags
        if (MedicalDeviceHelper.FindMedicalDevice(doctor, patient, JobDriver_UseSalineBag.JobDeviceDef, static patient => JobDriver_UseSalineBag.JobGetMedicalDeviceCountToFullyHeal(patient, fullyHeal: false), fromInventoryOnly: true) is Thing salineBag)
        {
            job = JobDriver_UseSalineBag.GetDispatcher(doctor, patient, salineBag, fromInventoryOnly: true, SalineTransfusionMode.Stabilize).CreateJob();
            return StartJobAndScheduleScan(doctor, patient, job);
        }
        // if no saline bags are available (if using them wouldn't be safe), use blood bags
        if (MedicalDeviceHelper.FindMedicalDevice(doctor, patient, JobDriver_UseBloodBag.JobDeviceDef, static patient => JobDriver_UseBloodBag.JobGetMedicalDeviceCountToFullyHeal(patient, fullyHeal: false), fromInventoryOnly: true) is Thing bloodBag)
        {
            job = JobDriver_UseBloodBag.GetDispatcher(doctor, patient, bloodBag, fromInventoryOnly: true, fullyHeal: false).CreateJob();
            return StartJobAndScheduleScan(doctor, patient, job);
        }
        // start normal vanilla treatment (only if the patient is downed because otherwise the patient will just get up and walk away)
        if (patient.Downed && patient.health.hediffSet.hediffs.Any(static hediff => hediff.TendableNow()))
        {
            Thing medicine = HealthAIUtility.FindBestMedicine(doctor, patient, onlyUseInventory: true);
            job = JobMaker.MakeJob(JobDefOf.TendPatient, patient);
            // re-scan after every single tend to prioritize conditions like cardiac arrest as they arrise
            job.endAfterTendedOnce = true;
            return StartJobAndScheduleScan(doctor, patient, job);
        }
        // since we finished vanilla treatment, we can now remove the tourniquets again
        if (bleedRateTotal < Mathf.Epsilon && patient.health.hediffSet.hediffs.Find(static hediff => hediff.def == KnownHediffDefOf.TourniquetApplied) is Hediff tourniquetApplied)
        {
            job = JobDriver_RemoveTourniquetSafely.GetDispatcher(doctor, patient, tourniquetApplied.Part).CreateJob();
            return StartJobAndScheduleScan(doctor, patient, job);
        }
        return false;
    }

    private static bool StartJobAndScheduleScan(Pawn doctor, Pawn patient, Job job)
    {
        bool requiresScheduling = false;
        if (doctor.jobs.TryTakeOrderedJob(job, requestQueueing: false))
        {
            requiresScheduling = true;
        }
        Job scan = new JobDescriptor(doctor, patient).CreateJob();
        doctor.jobs.TryTakeOrderedJob(scan, requestQueueing: requiresScheduling);
        return true;
    }

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient) => new JobDescriptor(doctor, patient);

    private readonly struct JobDescriptor(Pawn doctor, Pawn patient) : IJobDescriptor
    {
        public readonly Job CreateJob()
        {
            Job job = JobMaker.MakeJob(KnownJobDefOf.ProvideFirstAid, patient);
            job.count = 1;
            return job;
        }

        public readonly void StartJob()
        {
            Job job = CreateJob();
            doctor.jobs.TryTakeOrderedJob(job);
        }
    }
}
