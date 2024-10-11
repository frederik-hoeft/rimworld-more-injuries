using MoreInjuries.AI;
using MoreInjuries.KnownDefs;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.CardiacArrest;

public class JobDriver_UseDefibrillator : JobDriver_UseMedicalDevice
{
    public const string JOB_LABEL = "Use defibrillator";

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

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        Hediff? heartAttack = patient.health.hediffSet.hediffs.Find(hediff => hediff.def == KnownHediffDefOf.HeartAttack);
        float doctorSkill = doctor.skills.GetSkill(SkillDefOf.Medicine).Level;
        // global dice roll to ensure consistency between HeartAttack and CardiacArrest treatment outcomes
        bool success = Rand.Chance(Mathf.Max(MoreInjuriesMod.Settings.DefibrillatorMinimumSuccessRate, doctorSkill / 8f));
        if (heartAttack is not null && success)
        {
            patient.health.RemoveHediff(heartAttack);
        }
        // only continue if the feature is enabled
        if (success && MoreInjuriesMod.Settings.EnableCardiacArrestOnHighBloodLoss)
        {
            Hediff? cardiacArrest = patient.health.hediffSet.hediffs.Find(hediff => hediff.def == KnownHediffDefOf.CardiacArrest && hediff.CurStageIndex == 0);
            if (cardiacArrest is not null)
            {
                patient.health.RemoveHediff(cardiacArrest);
            }
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
                s_transientJobParameters.Add(job, new ExtendedJobParameters(FromInventoryOnly: true));
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
