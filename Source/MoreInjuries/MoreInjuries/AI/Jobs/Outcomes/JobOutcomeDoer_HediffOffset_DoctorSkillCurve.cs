using MoreInjuries.Extensions;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Text;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[XmlBindable]
public sealed partial class JobOutcomeDoer_HediffOffset_DoctorSkillCurve : JobOutcomeDoer_HediffOffsetBase
{
    [XmlBinding("minSeverityOffsetByDoctorSkill")]
    private partial SimpleCurve MinSeverityOffsetByDoctorSkill { get; }

    [XmlBinding("maxSeverityOffsetByDoctorSkill")]
    private partial SimpleCurve? MaxSeverityOffsetByDoctorSkill { get; }

    protected override float GetSeverityOffset(Pawn doctor, Pawn patient, Thing? device)
    {
        float doctorSkill = doctor.GetMedicalSkillLevelOrDefault();
        float minOffset = MinSeverityOffsetByDoctorSkill.Evaluate(doctorSkill);
        if (MaxSeverityOffsetByDoctorSkill is not { } maxOffsetCurve)
        {
            // if maxSeverityOffsetByDoctorSkill is not defined, use minSeverityOffsetByDoctorSkill
            return minOffset;
        }
        float maxOffset = maxOffsetCurve.Evaluate(doctorSkill);
        return Rand.Range(minOffset, maxOffset);
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(base.ToString());
        sb.Append(" with doctor skill curve offsets: min=");
        AddCurvePoints(MinSeverityOffsetByDoctorSkill, sb);
        sb.Append(", max=");
        AddCurvePoints(MaxSeverityOffsetByDoctorSkill, sb);
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
