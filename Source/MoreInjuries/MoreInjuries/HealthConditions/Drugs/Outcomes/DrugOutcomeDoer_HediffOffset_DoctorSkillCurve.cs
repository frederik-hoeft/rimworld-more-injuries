using MoreInjuries.BuildIntrinsics;
using MoreInjuries.Extensions;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs.Outcomes;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public sealed class DrugOutcomeDoer_HediffOffset_DoctorSkillCurve : DrugOutcomeDoer_HediffOffsetBase
{
    // don't rename this field. XML defs depend on this name
    private readonly SimpleCurve minSeverityOffsetByDoctorSkill = default!;
    // don't rename this field. XML defs depend on this name
    private readonly SimpleCurve? maxSeverityOffsetByDoctorSkill = default!;

    protected override float GetSeverityOffset(Pawn doctor, Pawn patient, Thing? device)
    {
        float doctorSkill = doctor.GetMedicalSkillLevelOrDefault();
        float minOffset = minSeverityOffsetByDoctorSkill.Evaluate(doctorSkill);
        if (maxSeverityOffsetByDoctorSkill is null)
        {
            // if maxSeverityOffsetByDoctorSkill is not defined, use minSeverityOffsetByDoctorSkill
            return minOffset;
        }   
        float maxOffset = maxSeverityOffsetByDoctorSkill.Evaluate(doctorSkill);
        return Rand.Range(minOffset, maxOffset);
    }
}