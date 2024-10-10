using MoreInjuries.AI;
using MoreInjuries.KnownDefs;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.Choking;

public class JobDriver_UseSuctionDevice : JobDriver_UseMedicalDevice_TargetsHediffDefs
{
    public const string JOB_LABEL = "Clear airways";

    public static HediffDef[] TargetHediffDefs { get; } = [KnownHediffDefOf.ChokingOnBlood];

    protected override HediffDef[] HediffDefs => TargetHediffDefs;

    protected override ThingDef DeviceDef => KnownThingDefOf.SuctionDevice;

    protected override bool RequiresDevice => true;

    protected override int BaseTendDuration => 600;

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        Hediff? choking = patient.health.hediffSet.hediffs.Find(hediff => hediff.def == KnownHediffDefOf.ChokingOnBlood);
        float doctorSkill = doctor.skills.GetSkill(SkillDefOf.Medicine).Level;
        bool success = Rand.Chance(Mathf.Max(MoreInjuriesMod.Settings.SuctionDeviceMinimumSuccessRate, doctorSkill / 8f));
        if (choking is not null && success)
        {
            patient.health.RemoveHediff(choking);
        }
    }

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device, bool fromInventoryOnly = false) => 
        new JobDescriptor(doctor, patient, device, fromInventoryOnly);

    public class JobDescriptor(Pawn doctor, Pawn patient, Thing device, bool fromInventoryOnly) : IJobDescriptor
    {
        public Job CreateJob()
        {
            Job job = JobMaker.MakeJob(KnownJobDefOf.UseSuctionDevice, patient, device);
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
