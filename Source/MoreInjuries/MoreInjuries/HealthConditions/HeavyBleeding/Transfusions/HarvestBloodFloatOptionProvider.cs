using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

internal class HarvestBloodFloatOptionProvider(InjuryWorker parent) : ICompFloatMenuOptionsHandler
{
    public bool IsEnabled => true;

    private static string? GetReasonForDisabledProcedure(Pawn doctor, string jobTitle)
    {
        if (!doctor.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
        {
            return $"{jobTitle}: {doctor} is incapable of manipulation";
        }
        if (!doctor.health.capacities.CapableOf(PawnCapacityDefOf.Sight))
        {
            return $"{jobTitle}: {doctor} is blind";
        }
        return null;
    }

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        Pawn patient = parent.Target;
        if (!builder.Keys.Contains(UITreatmentOption.HarvestBlood) && selectedPawn != patient && (patient.Downed || patient.IsPrisoner))
        {
            builder.Keys.Add(UITreatmentOption.HarvestBlood);
            int doctorSkill = selectedPawn.skills.GetSkill(SkillDefOf.Medicine).Level;
            string jobLabel = string.Format(JobDriver_HarvestBlood.JOB_LABEL, patient.LabelShort);
            if (doctorSkill < 6)
            {
                if (selectedPawn.Drafted)
                {
                    builder.Options.Add(new FloatMenuOption($"{jobLabel}: missing required medical skill (6)", null));
                }
                return;
            }
            if (GetReasonForDisabledProcedure(selectedPawn, jobLabel) is string failure)
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
            }
            else
            {
                builder.Options.Add(new FloatMenuOption(jobLabel, JobDriver_HarvestBlood.GetDispatcher(selectedPawn, patient).StartJob));
            }
        }
    }
}
