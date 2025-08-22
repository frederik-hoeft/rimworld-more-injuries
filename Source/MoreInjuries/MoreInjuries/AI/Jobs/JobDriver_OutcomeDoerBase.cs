using MoreInjuries.AI.Jobs.Outcomes;
using MoreInjuries.Extensions;
using Verse;
using Verse.AI;

namespace MoreInjuries.AI.Jobs;

public abstract class JobDriver_OutcomeDoerBase : JobDriver_UseMedicalDevice
{
    protected sealed override bool RequiresDevice => true;

    protected override bool ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        if (device is not { Destroyed: false })
        {
            Logger.Error($"failed to apply drug because the device was destroyed or null. What's going on?");
            EndJobWith(JobCondition.Incompletable);
            return false;
        }
        if (device.def.GetModExtension<JobOutcomeProperties_ModExtension>() is not JobOutcomeProperties_ModExtension { OutcomeDoers: { Count: > 0 } outcomeDoers })
        {
            Logger.ConfigError($"failed to apply drug because the device has no {nameof(JobOutcomeProperties_ModExtension)} or has no outcome doers defined for {device.def.defName}");
            EndJobWith(JobCondition.Incompletable);
            return false;
        }
        device.DecreaseStack();
        foreach (JobOutcomeDoer doer in outcomeDoers)
        {
            bool success = doer.TryDoOutcome(doctor, patient, device);
            if (!success)
            {
                Logger.Error($"failed to apply injector with outcome doer {doer.GetType().Name}");
                EndJobWith(JobCondition.Incompletable);
                return false;
            }
        }
        return true;
    }
}