using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Localization;
using MoreInjuries.Things;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

internal class UseSalineBagFloatOptionProvider(InjuryWorker parent) : ICompFloatMenuOptionsHandler
{
    public bool IsEnabled => true;

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        Pawn patient = parent.Target;
        if (!builder.Keys.Contains(UITreatmentOption.UseSalineBag) && selectedPawn.Drafted && JobDriver_UseSalineBag.JobGetMedicalDeviceCountToFullyHeal(patient, fullyHeal: true) > 0)
        {
            builder.Keys.Add(UITreatmentOption.UseSalineBag);
            if (!KnownResearchProjectDefOf.BasicFirstAid.IsFinished)
            {
                return;
            }
            if (MedicalDeviceHelper.GetCauseForDisabledProcedure(selectedPawn, patient, JobDriver_UseSalineBag.JOB_LABEL_KEY) is { FailureReason: string failure })
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, JobDriver_UseSalineBag.JobDeviceDef, fromInventoryOnly: true) is Thing inventoryThing)
            {
                if (JobDriver_UseSalineBag.JobGetMedicalDeviceCountToFullyHeal(patient, fullyHeal: false) > 0)
                {
                    builder.Options.Add(new FloatMenuOption(Named.Keys.Procedure_FromInventory_Stabilize.Translate(JobDriver_UseSalineBag.JOB_LABEL_KEY.Translate()), 
                        JobDriver_UseSalineBag.GetDispatcher(selectedPawn, patient, inventoryThing, fromInventoryOnly: true, fullyHeal: false).StartJob));
                }
                builder.Options.Add(new FloatMenuOption(Named.Keys.Procedure_FromInventory_FullyHeal.Translate(JobDriver_UseSalineBag.JOB_LABEL_KEY.Translate()), 
                    JobDriver_UseSalineBag.GetDispatcher(selectedPawn, patient, inventoryThing, fromInventoryOnly: true, fullyHeal: true).StartJob));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, JobDriver_UseSalineBag.JobDeviceDef) is not Thing thing)
            {
                builder.Options.Add(new FloatMenuOption("MI_UseSalineBagFailed_Unavailable".Translate(JobDriver_UseSalineBag.JOB_LABEL_KEY.Translate()), null));
            }
            else
            {
                if (JobDriver_UseSalineBag.JobGetMedicalDeviceCountToFullyHeal(patient, fullyHeal: false) > 0)
                {
                    builder.Options.Add(new FloatMenuOption(Named.Keys.Procedure_Stabilize.Translate(JobDriver_UseSalineBag.JOB_LABEL_KEY.Translate()), 
                        JobDriver_UseSalineBag.GetDispatcher(selectedPawn, patient, thing, fromInventoryOnly: false, fullyHeal: false).StartJob));
                }
                builder.Options.Add(new FloatMenuOption(Named.Keys.Procedure_FullyHeal.Translate(JobDriver_UseSalineBag.JOB_LABEL_KEY.Translate()), 
                    JobDriver_UseSalineBag.GetDispatcher(selectedPawn, patient, thing, fromInventoryOnly: false, fullyHeal: true).StartJob));
            }
        }
    }
}
