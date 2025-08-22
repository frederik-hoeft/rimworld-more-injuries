using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Extensions;
using MoreInjuries.Localization;
using MoreInjuries.Things;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

internal class HarvestBloodFloatOptionProvider(InjuryWorker parent) : ICompFloatMenuOptionsHandler
{
    public bool IsEnabled => true;

    private static string? GetReasonForDisabledProcedure(Pawn doctor, string jobTitleKey, Pawn patient)
    {
        if (!doctor.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
        {
            return Named.Keys.ProcedureFailed_IncapableOfManipulation.Translate(jobTitleKey.Translate(patient.Named(Named.Params.PATIENT), doctor.Named(Named.Params.DOCTOR)));
        }
        if (!doctor.health.capacities.CapableOf(PawnCapacityDefOf.Sight))
        {
            return Named.Keys.ProcedureFailed_IncapableOfSight.Translate(jobTitleKey.Translate(patient.Named(Named.Params.PATIENT), doctor.Named(Named.Params.DOCTOR)));
        }
        if (!doctor.CanReachPatient(patient))
        {
            return Named.Keys.ProcedureFailed_NoPath.Translate(jobTitleKey.Translate(patient.Named(Named.Params.PATIENT), doctor.Named(Named.Params.DOCTOR)));
        }
        return null;
    }

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        Pawn patient = parent.Pawn;
        if (!builder.Keys.Contains(UITreatmentOption.HarvestBlood) && selectedPawn != patient && (patient.Downed || patient.IsPrisoner) && JobDriver_HarvestBlood.JobCanTreat(patient))
        {
            builder.Keys.Add(UITreatmentOption.HarvestBlood);
            if (!KnownResearchProjectDefOf.BasicFirstAid.IsFinished)
            {
                return;
            }
            int doctorSkill = selectedPawn.GetMedicalSkillLevelOrDefault();
            int requiredSkill = MoreInjuriesMod.Settings.BloodTransfusionHarvestMinimumSkill;
            string jobLabel = string.Format(JobDriver_HarvestBlood.JOB_LABEL_KEY, patient.LabelShort);
            if (doctorSkill < requiredSkill)
            {
                if (selectedPawn.Drafted)
                {
                    builder.Options.Add(new FloatMenuOption("MI_HarvestBloodFailed_MissingMedicalSkill".Translate(
                        JobDriver_HarvestBlood.JOB_LABEL_KEY.Translate(patient.Named(Named.Params.PATIENT)), requiredSkill), null));
                }
                return;
            }
            if (GetReasonForDisabledProcedure(selectedPawn, JobDriver_HarvestBlood.JOB_LABEL_KEY, patient) is string failure)
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
            }
            else
            {
                builder.Options.Add(new FloatMenuOption(JobDriver_HarvestBlood.JOB_LABEL_KEY.Translate(patient.Named(Named.Params.PATIENT)), JobDriver_HarvestBlood.GetDispatcher(selectedPawn, patient).StartJob));
            }
        }
    }
}
