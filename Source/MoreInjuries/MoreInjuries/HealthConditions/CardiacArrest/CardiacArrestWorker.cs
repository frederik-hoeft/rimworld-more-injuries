using MoreInjuries.KnownDefs;
using MoreInjuries.Things;
using Verse;

namespace MoreInjuries.HealthConditions.CardiacArrest;

public class CardiacArrestWorker(MoreInjuryComp parent) : InjuryWorker(parent), ICompFloatMenuOptionsHandler
{
    public override bool IsEnabled => true; // because we allow the base-game HeartAttack to be treated

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        Pawn patient = Target;
        if (selectedPawn == patient || !selectedPawn.Drafted)
        {
            return;
        }
        // only show option when drafted
        if (!builder.Keys.Contains(UITreatmentOption.PerformCpr) && patient.health.hediffSet.hediffs.Any(hediff => Array.IndexOf(JobDriver_PerformCpr.TargetHediffDefs, hediff.def) != -1))
        {
            builder.Keys.Add(UITreatmentOption.PerformCpr);
            if (MedicalDeviceHelper.GetReasonForDisabledProcedure(selectedPawn, patient, JobDriver_PerformCpr.JOB_LABEL) is string failure)
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
            }
            else
            {
                builder.Options.Add(new FloatMenuOption(JobDriver_PerformCpr.JOB_LABEL, JobDriver_PerformCpr.GetDispatcher(selectedPawn, patient).StartJob));
            }
        }
        if (!builder.Keys.Contains(UITreatmentOption.UseDefibrillator) && patient.health.hediffSet.hediffs.Any(JobDriver_UseDefibrillator.JobCanTreat))
        {
            builder.Keys.Add(UITreatmentOption.UseDefibrillator);
            if (MedicalDeviceHelper.GetReasonForDisabledProcedure(selectedPawn, patient, JobDriver_UseDefibrillator.JOB_LABEL) is string failure)
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, KnownThingDefOf.Defibrillator, JobDriver_UseDefibrillator.TargetHediffDefs) is not Thing defibrillator)
            {
                builder.Options.Add(new FloatMenuOption($"{JobDriver_UseDefibrillator.JOB_LABEL}: no defibrillator available", null));
            }
            else
            {
                builder.Options.Add(new FloatMenuOption(JobDriver_UseDefibrillator.JOB_LABEL, JobDriver_UseDefibrillator.GetDispatcher(selectedPawn, patient, defibrillator).StartJob));
            }
        }
    }
}
