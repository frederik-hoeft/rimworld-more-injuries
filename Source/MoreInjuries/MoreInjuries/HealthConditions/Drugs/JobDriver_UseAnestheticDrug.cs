using MoreInjuries.Extensions;
using RimWorld;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs;

public abstract class JobDriver_UseAnestheticDrug : JobDriver_UseMedicalDrug
{
    protected override int CalculateTendDuration()
    {
        Pawn doctor = Doctor;
        Pawn patient = Patient;
        if (!patient.IsActivelyHostileTo(doctor))
        {
            return base.CalculateTendDuration();
        }
        // if the patient is actively resisting, it is more difficult to apply the anesthetic
        // scaled linearly from 1x to 3x duration based on the doctor's melee skill
        float doctorMeleeSkill;
        if (doctor.skills?.GetSkill(SkillDefOf.Melee) is { Level: var skillLevel })
        {
            doctorMeleeSkill = skillLevel;
        }
        else
        {
            doctorMeleeSkill = 0f;
        }
        float meleeFactor = Mathf.Clamp01(doctorMeleeSkill / 20f);
        float resistanceFactor = 1f + ((1f - meleeFactor) * 2f);
        int baseDuration = base.CalculateTendDuration();
        int duration = (int)(baseDuration * resistanceFactor);
        Logger.LogDebug($"Patient {patient.Name} is actively hostile to doctor {doctor.Name}. Increasing anesthetic application duration from {baseDuration} to {duration} (resistance factor: {resistanceFactor:0.00})");
        return duration;
    }
}
