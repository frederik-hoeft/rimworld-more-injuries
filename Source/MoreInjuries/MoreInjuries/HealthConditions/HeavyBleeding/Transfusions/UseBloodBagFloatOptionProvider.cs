using MoreInjuries.Things;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

internal class UseBloodBagFloatOptionProvider(InjuryWorker parent) : ICompFloatMenuOptionsHandler
{
    public bool IsEnabled => true;

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        Pawn patient = parent.Target;
        if (!builder.Keys.Contains(UITreatmentOption.UseBloodBag) && selectedPawn.Drafted && JobDriver_UseBloodBag.JobGetMedicalDeviceCountToFullyHeal(patient, fullyHeal: true) > 0)
        {
            builder.Keys.Add(UITreatmentOption.UseBloodBag);
            if (MedicalDeviceHelper.GetReasonForDisabledProcedure(selectedPawn, patient, JobDriver_UseBloodBag.JOB_LABEL) is string failure)
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, JobDriver_UseBloodBag.JobDeviceDef, fromInventoryOnly: true) is Thing inventoryThing)
            {
                if (JobDriver_UseBloodBag.JobGetMedicalDeviceCountToFullyHeal(patient, fullyHeal: false) > 0)
                {
                    builder.Options.Add(new FloatMenuOption($"{JobDriver_UseBloodBag.JOB_LABEL} (from inventory, stabilize)", JobDriver_UseBloodBag.GetDispatcher(selectedPawn, patient, inventoryThing, fromInventoryOnly: true, fullyHeal: false).StartJob));
                }
                builder.Options.Add(new FloatMenuOption($"{JobDriver_UseBloodBag.JOB_LABEL} (from inventory, fully heal)", JobDriver_UseBloodBag.GetDispatcher(selectedPawn, patient, inventoryThing, fromInventoryOnly: true, fullyHeal: true).StartJob));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, JobDriver_UseBloodBag.JobDeviceDef) is not Thing thing)
            {
                builder.Options.Add(new FloatMenuOption($"{JobDriver_UseBloodBag.JOB_LABEL}: no blood bags available", null));
            }
            else
            {
                if (JobDriver_UseBloodBag.JobGetMedicalDeviceCountToFullyHeal(patient, fullyHeal: false) > 0)
                {
                    builder.Options.Add(new FloatMenuOption($"{JobDriver_UseBloodBag.JOB_LABEL} (stabilize)", JobDriver_UseBloodBag.GetDispatcher(selectedPawn, patient, thing, fromInventoryOnly: false, fullyHeal: false).StartJob));
                }
                builder.Options.Add(new FloatMenuOption($"{JobDriver_UseBloodBag.JOB_LABEL} (fully heal)", JobDriver_UseBloodBag.GetDispatcher(selectedPawn, patient, thing, fromInventoryOnly: false, fullyHeal: true).StartJob));
            }
        }
    }
}
