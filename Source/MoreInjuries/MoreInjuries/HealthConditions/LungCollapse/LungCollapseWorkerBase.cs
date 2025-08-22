using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Extensions;
using MoreInjuries.HealthConditions.Secondary;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.LungCollapse;

internal abstract class LungCollapseWorkerBase(MoreInjuryComp parent) : InjuryWorker(parent)
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableLungCollapse;

    protected void CollapseLung<TCause>(BodyPartRecord lung, params ReadOnlySpan<TCause> causes)
    {
        Pawn patient = Pawn;
        float clampedUpperBound = Mathf.Clamp(MoreInjuriesMod.Settings.LungCollapseMaxSeverityRoot, 0.1f, 1.0f);
        float factor = Rand.Range(0.1f, clampedUpperBound);
        // we scale the severity by the square of the factor to make it more likely to be low, but allow for high values with a small chance
        float severityIncrease = factor * factor;
        if (!patient.health.hediffSet.TryGetFirstHediffMatchingPart(lung, KnownHediffDefOf.LungCollapse, out Hediff? lungCollapse))
        {
            lungCollapse = HediffMaker.MakeHediff(KnownHediffDefOf.LungCollapse, patient, lung);
            lungCollapse.Severity = severityIncrease;
            patient.health.AddHediff(lungCollapse);
        }
        else
        {
            lungCollapse.Severity += severityIncrease;
        }
        if (causes.Length > 0 && lungCollapse.TryGetComp<HediffComp_CausedBy>() is { } causedBy)
        {
            for (int i = 0; i < causes.Length; ++i)
            {
                TCause? cause = causes[i];
                if (cause is string label)
                {
                    causedBy.AddCause(label);
                }
                else if (cause is Hediff hediff)
                {
                    causedBy.AddCause(hediff);
                }
                else
                {
                    Logger.ConfigError($"Unabel to handle unknown cause of type {typeof(TCause)} for {nameof(HediffComp_CausedBy)}");
                }
            }
        }
        Logger.LogDebug($"Applied lung collapse to {lung.Label} of {patient.Name}");
    }
}
