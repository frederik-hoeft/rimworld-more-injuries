using MoreInjuries.BuildIntrinsics;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.Injectors;

[SuppressMessage("Style", "IDE0032:Use auto property", Justification = Justifications.XML_DEF_REQUIRES_FIELD)]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class InjectionOutcomeDoer_HediffOffset : InjectionOutcomeDoer
{
    // don't rename this field. XML defs depend on this name
    private readonly HediffDef hediffDef = default!;

    // don't rename this field. XML defs depend on this name
    private readonly float severityOffset = default;

    public HediffDef HediffDef => hediffDef;

    public float SeverityOffset => severityOffset;

    public override bool TryDoOutcome(Pawn doctor, Pawn patient, Thing? device)
    {
        Hediff? hediff = patient.health.hediffSet.GetFirstHediffOfDef(HediffDef);
        if (hediff is null && SeverityOffset > 0)
        {
            hediff = HediffMaker.MakeHediff(HediffDef, patient);
            patient.health.AddHediff(hediff);
        }
        if (hediff is not null)
        {
            float severity = hediff.Severity + SeverityOffset;
            if (severity <= 0f)
            {
                patient.health.RemoveHediff(hediff);
                return true;
            }
            hediff.Severity = Mathf.Min(severity, HediffDef.maxSeverity);
        }
        return true;
    }
}