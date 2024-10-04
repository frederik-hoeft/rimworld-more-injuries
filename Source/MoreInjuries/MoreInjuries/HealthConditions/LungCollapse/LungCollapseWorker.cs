using MoreInjuries.Extensions;
using MoreInjuries.KnownDefs;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.LungCollapse;

public class LungCollapseWorker(InjuryComp parent) : InjuryWorker(parent), IPostPostApplyDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableLungCollapse;

    public void PostPostApplyDamage(ref readonly DamageInfo dinfo)
    {
        Pawn patient = Target;
        if ((dinfo.Def == DamageDefOf.Bomb || dinfo.Def?.defName is "Thermobaric") && Rand.Chance(MoreInjuriesMod.Settings.LungCollapseChanceOnDamage))
        {
            IEnumerable<BodyPartRecord> lungs = patient.health.hediffSet.GetNotMissingParts().Where(bodyPart => bodyPart.def == BodyPartDefOf.Lung);
            float clampedUpperBound = Mathf.Clamp(MoreInjuriesMod.Settings.LungCollapseMaxSeverityRoot, 0.1f, 1.0f);
            foreach (BodyPartRecord lung in lungs)
            {
                if (!patient.health.hediffSet.TryGetFirstHediffMatchingPart(lung, KnownHediffDefOf.LungCollapse, out Hediff? lungCollapse))
                {
                    lungCollapse = HediffMaker.MakeHediff(KnownHediffDefOf.LungCollapse, patient, lung);
                    patient.health.AddHediff(lungCollapse);
                }
                float factor = Rand.Range(0.1f, clampedUpperBound);
                // we scale the severity by the square of the factor to make it more likely to be low, but allow for high values with a small chance
                lungCollapse!.Severity = factor * factor;
            }
        }
    }
}
