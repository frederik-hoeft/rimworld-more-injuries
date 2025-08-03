using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Extensions;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.LungCollapse;

internal sealed class LungCollapseWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostPostApplyDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableLungCollapse;

    public void PostPostApplyDamage(ref readonly DamageInfo dinfo)
    {
        Pawn patient = Target;
        if (dinfo.Def == DamageDefOf.Bomb || dinfo.Def?.defName is "Thermobaric")
        {
            bool chanceLung1 = Rand.Chance(MoreInjuriesMod.Settings.LungCollapseChanceOnDamage);
            bool chanceLung2 = Rand.Chance(MoreInjuriesMod.Settings.LungCollapseChanceOnDamage);
            if (!chanceLung1 && !chanceLung2)
            {
                // early exit if both chances fail
                return;
            }
            IEnumerable<BodyPartRecord> lungs = patient.health.hediffSet.GetNotMissingParts().Where(static bodyPart => bodyPart.def == BodyPartDefOf.Lung);
            float clampedUpperBound = Mathf.Clamp(MoreInjuriesMod.Settings.LungCollapseMaxSeverityRoot, 0.1f, 1.0f);
            ReadOnlySpan<bool> chances = [chanceLung1, chanceLung2];
            int i = 0;
            foreach (BodyPartRecord lung in lungs)
            {
                if (i >= chances.Length)
                {
                    Logger.Warning($"LungCollapseWorker.PostPostApplyDamage: pawn {patient.Name} has more than 2 lungs? What's going on? Ignoring the extra lungs...");
                    break;
                }
                if (!chances[i])
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
                ++i;
            }
        }
    }
}