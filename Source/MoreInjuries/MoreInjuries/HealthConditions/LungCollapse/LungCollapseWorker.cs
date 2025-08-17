using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Extensions;
using MoreInjuries.HealthConditions.LungCollapse.DamageHandlers;
using MoreInjuries.HealthConditions.Secondary;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.LungCollapse;

internal sealed class LungCollapseWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostApplyDamageToPartHandler
{
    private static readonly ILungCollapseHandler[] s_lungCollapseHandlers =
    [
        new ThermobaricDamageHandler(),
        new PerforatingDamageHandler(),
    ];

    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableLungCollapse 
        && (MoreInjuriesMod.Settings.LungCollapseChanceOnThermobaricDamage > 0f 
            || MoreInjuriesMod.Settings.LungCollapseChanceOnPerforatingDamage > 0f);

    public void ApplyDamageToPart(ref readonly DamageInfo dinfo, Pawn pawn, DamageWorker.DamageResult result)
    {
        Logger.LogDebug($"{pawn.Label} took damage to {dinfo.HitPart?.Label ?? "null"}. Other things: {string.Join(", ", result.hediffs?.Select(static hediff => $"'{hediff.Label} : {hediff.Part.Label}") ?? [])}'" +
            $"Also: {string.Join(", ", result.parts.Select(p => p.Label))}");

        Pawn patient = Pawn;
        LungCollapseEvaluationContext context = default;
        bool handled = false;
        List<Hediff>? causes = null;
        foreach (ILungCollapseHandler handler in s_lungCollapseHandlers)
        {
            handled |= handler.EvaluateDamageChances(this, in dinfo, ref context, ref causes);
        }
        if (!handled)
        {
            // early exit if both chances fail
            return;
        }
        bool chanceLung1 = Rand.Chance(context.ChanceLung0);
        bool chanceLung2 = Rand.Chance(context.ChanceLung1);
        if (!chanceLung1 && !chanceLung2)
        {
            // early exit if both chances fail
            return;
        }
        Span<BodyPartRecord?> lungs = context.Lungs;
        if (context.Lung0 == null)
        {
            IEnumerable<BodyPartRecord> rawLungs = patient.health.hediffSet.GetNotMissingParts().Where(static bodyPart => bodyPart.def == BodyPartDefOf.Lung);
            int i = 0;
            foreach (BodyPartRecord lung in rawLungs)
            {
                if (i >= lungs.Length)
                {
                    break;
                }
                lungs[i++] = lung;
            }
        }
        float clampedUpperBound = Mathf.Clamp(MoreInjuriesMod.Settings.LungCollapseMaxSeverityRoot, 0.1f, 1.0f);
        ReadOnlySpan<bool> chances = [chanceLung1, chanceLung2];
        for (int i = 0; i < lungs.Length && i < chances.Length; ++i)
        {
            BodyPartRecord? lung = lungs[i];
            if (lung is null || !chances[i])
            {
                // skip this lung if the chance failed
                continue;
            }
            if (!patient.health.hediffSet.TryGetFirstHediffMatchingPart(lung, KnownHediffDefOf.LungCollapse, out Hediff? lungCollapse))
            {
                lungCollapse = HediffMaker.MakeHediff(KnownHediffDefOf.LungCollapse, patient, lung);
                patient.health.AddHediff(lungCollapse);
            }
            float factor = Rand.Range(0.1f, clampedUpperBound);
            // we scale the severity by the square of the factor to make it more likely to be low, but allow for high values with a small chance
            lungCollapse.Severity = factor * factor;
            if (causes is { Count: > 0 } && lungCollapse.TryGetComp<HediffComp_CausedBy>() is { } causedBy)
            {
                foreach (Hediff cause in causes)
                {
                    causedBy.AddCause(cause);
                }
            }
        }
    }
}