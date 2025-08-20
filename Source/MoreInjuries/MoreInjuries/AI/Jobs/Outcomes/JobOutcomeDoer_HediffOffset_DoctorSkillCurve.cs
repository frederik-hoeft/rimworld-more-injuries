using MoreInjuries.Extensions;
using System.Linq;
using System.Text;
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

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(base.ToString());
        sb.Append(" with doctor skill curve offsets: min=");
        AddCurvePoints(minSeverityOffsetByDoctorSkill, sb);
        sb.Append(", max=");
        AddCurvePoints(maxSeverityOffsetByDoctorSkill, sb);
        return sb.ToString();

        static void AddCurvePoints(SimpleCurve? curve, StringBuilder sb)
        {
            if (curve is { Points: { } points })
            {
                bool first = true;
                foreach (CurvePoint point in points)
                {
                    if (!first)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(point.ToString());
                    first = false;
                }
            }
            else
            {
                sb.Append("null");
            }
        }
    }
}