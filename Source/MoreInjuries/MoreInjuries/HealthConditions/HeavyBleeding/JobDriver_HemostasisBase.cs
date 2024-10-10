using MoreInjuries.AI;
using MoreInjuries.Debug;
using System.Linq;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.HeavyBleeding;

public abstract class JobDriver_HemostasisBase : JobDriver_UseMedicalDevice
{
    protected override bool RequiresDevice => true;

    protected override bool IsTreatable(Hediff hediff) => JobCanTreat(hediff);

    public static bool JobCanTreat(Hediff hediff) => hediff is BetterInjury
    {
        Part.depth: BodyPartDepth.Outside,
        Bleeding: true,
        IsCoagulationMultiplierApplied: false
    };

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        DebugAssert.NotNull(device, "Device cannot be null in JobDriver_HemostasisBase::ApplyDevice");

        Hediff? injury = patient.health.hediffSet.hediffs
            .Where(IsTreatable)
            .OrderByDescending(hediff => hediff.BleedRate)
            .FirstOrDefault();
        if (injury is not null && device?.def.GetModExtension<HemostasisModExtension>() is HemostasisModExtension extension)
        {
            BetterInjury betterInjury = (BetterInjury)injury;
            betterInjury.IsBase = false;
            betterInjury.CoagulationMultiplier = extension.CoagulationMultiplier;
            betterInjury.ReducedBleedRateTicksTotal = extension.DisappearsAfterTicks;
            betterInjury.IsCoagulationMultiplierApplied = true;
            device?.DecreaseStack();
        }
    }

    protected static IJobDescriptor GetDispatcher(JobDef jobDef, Pawn doctor, Pawn patient, Thing device, bool fromInventoryOnly) => 
        new JobDescriptor(jobDef, doctor, patient, device, fromInventoryOnly);

    public class JobDescriptor(JobDef jobDef, Pawn doctor, Pawn patient, Thing device, bool fromInventoryOnly) : IJobDescriptor
    {
        public Job CreateJob()
        {
            Job job = JobMaker.MakeJob(jobDef, patient, device);
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