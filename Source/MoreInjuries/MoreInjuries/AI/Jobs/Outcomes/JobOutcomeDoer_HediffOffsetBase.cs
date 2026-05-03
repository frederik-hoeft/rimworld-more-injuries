using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using UnityEngine;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[XmlSerializable]
public abstract partial class JobOutcomeDoer_HediffOffsetBase : JobOutcomeDoer
{
    [XmlMember("hediffDef")]
    public partial HediffDef HediffDef { get; }

    protected abstract float GetSeverityOffset(Pawn doctor, Pawn patient, Thing? device);

    protected override bool DoOutcome(Pawn doctor, Pawn patient, Thing? device)
    {
        HediffDef hediffDef = HediffDef;
        Hediff? hediff = patient.health.hediffSet.GetFirstHediffOfDef(hediffDef);
        float severityOffset = GetSeverityOffset(doctor, patient, device);
        Logger.LogDebug($"Calculating hediff {hediffDef.defName} ({hediff?.Severity.ToString() ?? "null"}) severity offset for {patient}: {severityOffset}");
        if (hediff is null && severityOffset > Mathf.Epsilon)
        {
            hediff = HediffMaker.MakeHediff(hediffDef, patient);
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
            hediff.Severity = Mathf.Min(severity, hediffDef.maxSeverity);
        }
        return true;
    }

    public override string ToString() => $"{GetType().Name}: {HediffDef.defName}";
}
