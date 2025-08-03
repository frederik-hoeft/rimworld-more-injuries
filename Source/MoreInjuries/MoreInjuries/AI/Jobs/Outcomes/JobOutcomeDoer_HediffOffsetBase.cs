using UnityEngine;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public abstract class JobOutcomeDoer_HediffOffsetBase : JobOutcomeDoer
{
    // don't rename this field. XML defs depend on this name
    private readonly HediffDef hediffDef = default!;

    public HediffDef HediffDef => hediffDef;

    protected abstract float GetSeverityOffset(Pawn doctor, Pawn patient, Thing? device);

    protected override bool DoOutcome(Pawn doctor, Pawn patient, Thing? device)
    {
        Hediff? hediff = patient.health.hediffSet.GetFirstHediffOfDef(HediffDef);
        float severityOffset = GetSeverityOffset(doctor, patient, device);
        Logger.LogDebug($"Calculating hediff {HediffDef.defName} ({hediff?.Severity.ToString() ?? "null"}) severity offset for {patient}: {severityOffset}");
        if (hediff is null && severityOffset > Mathf.Epsilon)
        {
            hediff = HediffMaker.MakeHediff(HediffDef, patient);
            patient.health.AddHediff(hediff);
            Logger.LogDebug($"Adding hediff {hediffDef.defName} to {patient}");
        }
        if (hediff is not null)
        {
            Logger.LogDebug($"Adjusting hediff {hediffDef.defName} (severity={hediff.Severity}) severity for {patient} by {severityOffset}");
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

    public override string ToString() => $"{GetType().Name}: {HediffDef.defName}";
}
