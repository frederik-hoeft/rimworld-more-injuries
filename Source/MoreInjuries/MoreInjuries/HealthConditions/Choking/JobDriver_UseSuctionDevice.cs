using MoreInjuries.AI.Audio;
using MoreInjuries.AI.Jobs;
using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Extensions;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.Choking;

public class JobDriver_UseSuctionDevice : JobDriver_UseMedicalDevice
{
    public const string JOB_LABEL_KEY = "MI_UseSuctionDevice";

    internal static SkillBasedOutcomeCurve OutcomeCurve { get; } = new(lower: (Min: 0.05f, Max: 0.5f), upper: (Min: 0.4f, Max: 0.6f));

    protected override ThingDef DeviceDef => KnownThingDefOf.SuctionDevice;

    protected override ISoundDefProvider<Pawn> SoundDefProvider => CachedSoundDefProvider.Of<Pawn>(KnownSoundDefOf.UseSuctionDevice);

    protected override bool RequiresDevice => true;

    protected override int BaseTendDuration => 600;

    protected override bool IsTreatable(Hediff hediff) => JobCanTreat(hediff);

    public static bool JobCanTreat(Hediff hediff) => JobCanTreat(hediff, out _);

    public static bool JobCanTreat(Hediff hediff, [NotNullWhen(true)] out HediffComp_Choking? comp)
    {
        if (hediff.def == KnownHediffDefOf.ChokingOnBlood
            && hediff is HediffWithComps { } choking && choking.TryGetComp(out HediffComp_Choking chokingComp)
            && chokingComp.FluidBuildup > 0f)
        {
            comp = chokingComp;
            return true;
        }
        comp = null;
        return false;
    }

    // TODO [BREAKING]: make suction device: 1. breakable, 2. stackable
    // TODO [BREAKING]: promote fluid burden to a dedicated (hidden) hediff, and only modify that instead of the choking severity directly (addressing the cause, not the symptom)
    protected override bool ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        foreach (Hediff hediff in patient.health.hediffSet.hediffs)
        {
            if (JobCanTreat(hediff, out HediffComp_Choking? comp))
            {
                ApplySuction(doctor, comp);
                return true;
            }
        }
        return false;
    }

    private void ApplySuction(Pawn doctor, HediffComp_Choking comp)
    {
        float playerScale = MoreInjuriesMod.Settings.SuctionDeviceEffectivenessMultiplier;

        float oldFluidBuildup = comp.FluidBuildup;
        float severityReduction = OutcomeCurve.Evaluate(doctor, comp, job.def, scale: playerScale).RandomInRange;
        float removedFluidBuildup = comp.ReduceFluidBuildup(severityReduction);

        Logger.LogDebug(
            $"Airway suction performed by {doctor.NameShortColored} on {comp.Pawn.NameShortColored}: " +
            $"medical skill={doctor.GetMedicalSkillLevelOrDefault():F1}, attempted removal={severityReduction.ToStringPercent("F1")}, " +
            $"removed={removedFluidBuildup.ToStringPercent("F1")}, fluid buildup={oldFluidBuildup.ToStringPercent("F1")}->{comp.FluidBuildup.ToStringPercent("F1")}");
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
