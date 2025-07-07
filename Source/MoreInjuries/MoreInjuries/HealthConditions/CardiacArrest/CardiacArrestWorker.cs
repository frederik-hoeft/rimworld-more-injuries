﻿using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Things;
using Verse;

namespace MoreInjuries.HealthConditions.CardiacArrest;

public sealed class CardiacArrestWorker(MoreInjuryComp parent) : InjuryWorker(parent), ICompFloatMenuOptionsHandler
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
        if (!builder.Keys.Contains(UITreatmentOption.PerformCpr) && patient.health.hediffSet.hediffs.Any(static hediff => Array.IndexOf(JobDriver_PerformCpr.TargetHediffDefs, hediff.def) != -1))
        {
            builder.Keys.Add(UITreatmentOption.PerformCpr);
            if (!KnownResearchProjectDefOf.Cpr.IsFinished)
            {
                return;
            }
            if (MedicalDeviceHelper.GetCauseForDisabledProcedure(selectedPawn, patient, JobDriver_PerformCpr.JOB_LABEL_KEY) is { FailureReason: string failure })
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
            }
            else
            {
                builder.Options.Add(new FloatMenuOption(JobDriver_PerformCpr.JOB_LABEL_KEY.Translate(), JobDriver_PerformCpr.GetDispatcher(selectedPawn, patient).StartJob));
            }
        }
        if (!builder.Keys.Contains(UITreatmentOption.UseDefibrillator) && patient.health.hediffSet.hediffs.Any(JobDriver_UseDefibrillator.JobCanTreat))
        {
            builder.Keys.Add(UITreatmentOption.UseDefibrillator);
            if (!KnownResearchProjectDefOf.EmergencyMedicine.IsFinished)
            {
                return;
            }
            if (MedicalDeviceHelper.GetCauseForDisabledProcedure(selectedPawn, patient, JobDriver_UseDefibrillator.JOB_LABEL_KEY) is { FailureReason: string failure })
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, KnownThingDefOf.Defibrillator, JobDriver_UseDefibrillator.TargetHediffDefs) is not Thing defibrillator)
            {
                builder.Options.Add(new FloatMenuOption("MI_UseDefibrillatorFailed_Unavailable".Translate(JobDriver_UseDefibrillator.JOB_LABEL_KEY.Translate()), null));
            }
            else
            {
                builder.Options.Add(new FloatMenuOption(JobDriver_UseDefibrillator.JOB_LABEL_KEY.Translate(), JobDriver_UseDefibrillator.GetDispatcher(selectedPawn, patient, defibrillator).StartJob));
            }
        }
    }
}