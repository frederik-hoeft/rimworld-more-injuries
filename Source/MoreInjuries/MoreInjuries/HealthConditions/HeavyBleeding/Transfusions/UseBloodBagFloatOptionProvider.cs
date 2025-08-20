using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Localization;
using MoreInjuries.Things;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

internal class UseBloodBagFloatOptionProvider(InjuryWorker parent) : ICompFloatMenuOptionsHandler
{
    public bool IsEnabled => true;

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        Pawn patient = parent.Pawn;
        if (!builder.Keys.Contains(UITreatmentOption.UseBloodBag) 
            && selectedPawn.Drafted 
            && JobDriver_UseBloodBag.JobGetMedicalDeviceCountToFullyHeal(patient, fullyHeal: true) > 0)
        {
            builder.Keys.Add(UITreatmentOption.UseBloodBag);
            if (!KnownResearchProjectDefOf.BasicFirstAid.IsFinished)
            {
                return;
            }
            if (MedicalDeviceHelper.GetCauseForDisabledProcedure(selectedPawn, patient, JobDriver_UseBloodBag.JOB_LABEL_KEY) is { FailureReason: string failure })
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, JobDriver_UseBloodBag.JobDeviceDef, fromInventoryOnly: true) is Thing inventoryThing)
            {
                if (JobDriver_UseBloodBag.JobGetMedicalDeviceCountToFullyHeal(patient, fullyHeal: false) > 0)
                {
                    builder.Options.Add(new FloatMenuOption(Named.Keys.Procedure_FromInventory_Stabilize.Translate(JobDriver_UseBloodBag.JOB_LABEL_KEY.Translate()), 
                        JobDriver_UseBloodBag.GetDispatcher(selectedPawn, patient, inventoryThing, fromInventoryOnly: true, fullyHeal: false).StartJob));
                }
                builder.Options.Add(new FloatMenuOption(Named.Keys.Procedure_FromInventory_FullyHeal.Translate(JobDriver_UseBloodBag.JOB_LABEL_KEY.Translate()), 
                    JobDriver_UseBloodBag.GetDispatcher(selectedPawn, patient, inventoryThing, fromInventoryOnly: true, fullyHeal: true).StartJob));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, JobDriver_UseBloodBag.JobDeviceDef) is not Thing thing)
            {
                builder.Options.Add(new FloatMenuOption("MI_UseBloodBagFailed_Unavailable".Translate(JobDriver_UseBloodBag.JOB_LABEL_KEY.Translate()), null));
            }
            else
            {
                if (JobDriver_UseBloodBag.JobGetMedicalDeviceCountToFullyHeal(patient, fullyHeal: false) > 0)
                {
                    builder.Options.Add(new FloatMenuOption(Named.Keys.Procedure_Stabilize.Translate(JobDriver_UseBloodBag.JOB_LABEL_KEY.Translate()), 
                        JobDriver_UseBloodBag.GetDispatcher(selectedPawn, patient, thing, fromInventoryOnly: false, fullyHeal: false).StartJob));
                }
                builder.Options.Add(new FloatMenuOption(Named.Keys.Procedure_FullyHeal.Translate(JobDriver_UseBloodBag.JOB_LABEL_KEY.Translate()), 
                    JobDriver_UseBloodBag.GetDispatcher(selectedPawn, patient, thing, fromInventoryOnly: false, fullyHeal: true).StartJob));
            }
        }
    }
}
