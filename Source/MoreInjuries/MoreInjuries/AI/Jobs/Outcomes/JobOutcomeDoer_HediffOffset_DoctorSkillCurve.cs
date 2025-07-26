using MoreInjuries.Extensions;
using System.Linq;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class JobOutcomeDoer_HediffOffset_DoctorSkillCurve : JobOutcomeDoer_HediffOffsetBase
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

    public override string ToString() => 
        $"{base.ToString()} with doctor skill curve offsets: min={string.Join(", ", minSeverityOffsetByDoctorSkill.Points.Select(point => point.ToString()))}, max={string.Join(", ", maxSeverityOffsetByDoctorSkill?.Points.Select(point => point.ToString()) ?? [])}";
}