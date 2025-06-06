﻿using MoreInjuries.AI;
using MoreInjuries.Extensions;
using MoreInjuries.KnownDefs;
using MoreInjuries.Things;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.CardiacArrest;

public class JobDriver_UseDefibrillator : JobDriver_UseMedicalDevice
{
    public const string JOB_LABEL_KEY = "MI_UseDefibrillator";

    public static HediffDef[] TargetHediffDefs { get; } = [KnownHediffDefOf.CardiacArrest, KnownHediffDefOf.HeartAttack];

    protected override bool RequiresDevice => true;

    protected override ThingDef DeviceDef => KnownThingDefOf.Defibrillator;

    protected override SoundDef SoundDef => KnownSoundDefOf.Defibrillator;

    protected override int BaseTendDuration => 180;

    protected override bool IsTreatable(Hediff hediff) => JobCanTreat(hediff);

    public static bool JobCanTreat(Hediff hediff) =>
        // must be white-listed ...
        Array.IndexOf(TargetHediffDefs, hediff.def) is int index && index >= 0
        // ... and not in the later stages of CardiacArrest
        && (hediff.def != KnownHediffDefOf.CardiacArrest || hediff.CurStageIndex == 0);

    protected override int GetMedicalDeviceCountToFullyHeal(Pawn patient)
    {
        // at max 1 (there is only one heart)
        if (patient.health.hediffSet.hediffs.Any(JobCanTreat))
        {
            return 1;
        }
        return 0;
    }

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        Hediff? heartAttack = patient.health.hediffSet.hediffs.Find(static hediff => hediff.def == KnownHediffDefOf.HeartAttack);
        float doctorSkill = doctor.GetMedicalSkillLevelOrDefault();
        // global dice roll to ensure consistency between HeartAttack and CardiacArrest treatment outcomes
        bool success = Rand.Chance(Mathf.Max(MoreInjuriesMod.Settings.DefibrillatorMinimumSuccessRate, doctorSkill / 8f));
        if (heartAttack is not null && success)
        {
            patient.health.RemoveHediff(heartAttack);
        }
        // only continue if the feature is enabled
        if (success && MoreInjuriesMod.Settings.EnableCardiacArrestOnHighBloodLoss)
        {
            Hediff? cardiacArrest = patient.health.hediffSet.hediffs.Find(static hediff => hediff.def == KnownHediffDefOf.CardiacArrest && hediff.CurStageIndex == 0);
            if (cardiacArrest is not null)
            {
                patient.health.RemoveHediff(cardiacArrest);
            }
        }
        if (device is not null)
        {
            ReusabilityUtility.TryDestroyReusableIngredient(device, doctor);
        }
    }

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device, bool fromInventoryOnly = false) => 
        new JobDescriptor(doctor, patient, device, fromInventoryOnly);

    public class JobDescriptor(Pawn doctor, Pawn patient, Thing device, bool fromInventoryOnly) : IJobDescriptor
    {
        public Job CreateJob()
        {
            Job job = JobMaker.MakeJob(KnownJobDefOf.UseDefibrillator, patient, device);
            job.count = 1;
            if (fromInventoryOnly)
            {
                ExtendedJobParameters parameters = ExtendedJobParameters.Create<ExtendedJobParameters>(doctor, fromInventoryOnly: true);
                job.source = parameters;
            }
            return job;
        }

        public void StartJob()
        {
            Job job = CreateJob();
            doctor.jobs.TryTakeOrderedJob(job);
        }
    }
}
