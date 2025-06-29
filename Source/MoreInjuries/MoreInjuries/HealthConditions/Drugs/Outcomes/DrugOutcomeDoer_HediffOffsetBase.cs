using MoreInjuries.BuildIntrinsics;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs.Outcomes;

[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public abstract class DrugOutcomeDoer_HediffOffsetBase : DrugOutcomeDoer
{
    // don't rename this field. XML defs depend on this name
    private readonly HediffDef hediffDef = default!;

    public HediffDef HediffDef => hediffDef;

    protected abstract float GetSeverityOffset(Pawn doctor, Pawn patient, Thing? device);

    protected override bool DoOutcome(Pawn doctor, Pawn patient, Thing? device)
    {
        Hediff? hediff = patient.health.hediffSet.GetFirstHediffOfDef(HediffDef);
        float severityOffset = GetSeverityOffset(doctor, patient, device);
        if (hediff is null && severityOffset > Mathf.Epsilon)
        {
            hediff = HediffMaker.MakeHediff(HediffDef, patient);
            patient.health.AddHediff(hediff);
        }
        if (hediff is not null)
        {
            float severity = hediff.Severity + severityOffset;
            if (severity <= Mathf.Epsilon)
            {
                patient.health.RemoveHediff(hediff);
                return true;
            }
            hediff.Severity = Mathf.Min(severity, HediffDef.maxSeverity);
        }
        return true;
    }
}
