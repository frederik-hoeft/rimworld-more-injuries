using MoreInjuries.KnownDefs;
using MoreInjuries.Things;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Bandages;

internal class BandageFloatOptionProvider(InjuryWorker parent) : ICompFloatMenuOptionsHandler
{
    public bool IsEnabled => true;

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        Pawn patient = parent.Target;
        if (!builder.Keys.Contains(UITreatmentOption.UseBandage) && selectedPawn.Drafted && patient.health.hediffSet.hediffs.Any(JobDriver_HemostasisBase.JobCanTreat))
        {
            builder.Keys.Add(UITreatmentOption.UseBandage);
            if (MedicalDeviceHelper.GetReasonForDisabledProcedure(selectedPawn, patient, JobDriver_UseBandage.JOB_LABEL) is string failure)
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, KnownThingDefOf.Bandage, JobDriver_HemostasisBase.JobCanTreat, fromInventoryOnly: true) is Thing inventoryThing)
            {
                builder.Options.Add(new FloatMenuOption($"{JobDriver_UseBandage.JOB_LABEL} (from inventory)", JobDriver_UseBandage.GetDispatcher(selectedPawn, patient, inventoryThing, fromInventoryOnly: true).StartJob));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, KnownThingDefOf.Bandage, JobDriver_HemostasisBase.JobCanTreat) is not Thing thing)
            {
                builder.Options.Add(new FloatMenuOption($"{JobDriver_UseBandage.JOB_LABEL}: no bandages available", null));
            }
            else
            {
                builder.Options.Add(new FloatMenuOption(JobDriver_UseBandage.JOB_LABEL, JobDriver_UseBandage.GetDispatcher(selectedPawn, patient, thing, fromInventoryOnly: false).StartJob));
            }
        }
    }
}
