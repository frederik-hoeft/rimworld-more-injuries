using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Localization;
using MoreInjuries.Things;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Bandages;

internal class BandageFloatOptionProvider(InjuryWorker parent) : ICompFloatMenuOptionsHandler
{
    public bool IsEnabled => true;

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        Pawn patient = parent.Pawn;
        if (!builder.Keys.Contains(UITreatmentOption.UseBandage) && selectedPawn.Drafted && patient.health.hediffSet.hediffs.Any(JobDriver_HemostasisBase.JobCanTreat))
        {
            builder.Keys.Add(UITreatmentOption.UseBandage);
            if (!KnownResearchProjectDefOf.BasicAnatomy.IsFinished)
            {
                return;
            }
            if (MedicalDeviceHelper.GetCauseForDisabledProcedure(selectedPawn, patient, JobDriver_UseBandage.JOB_LABEL_KEY) is { FailureReason: string failure })
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, KnownThingDefOf.Bandage, JobDriver_HemostasisBase.JobCanTreat, fromInventoryOnly: true) is Thing inventoryThing)
            {
                builder.Options.Add(new FloatMenuOption(Named.Keys.Procedure_FromInventory.Translate(JobDriver_UseBandage.JOB_LABEL_KEY.Translate()), JobDriver_UseBandage.GetDispatcher(selectedPawn, patient, inventoryThing, fromInventoryOnly: true).StartJob));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, KnownThingDefOf.Bandage, JobDriver_HemostasisBase.JobCanTreat) is not Thing thing)
            {
                builder.Options.Add(new FloatMenuOption("MI_UseBandagesFailed_Unavailable".Translate(JobDriver_UseBandage.JOB_LABEL_KEY.Translate()), null));
            }
            else
            {
                builder.Options.Add(new FloatMenuOption(JobDriver_UseBandage.JOB_LABEL_KEY.Translate(), JobDriver_UseBandage.GetDispatcher(selectedPawn, patient, thing, fromInventoryOnly: false).StartJob));
            }
        }
    }
}
