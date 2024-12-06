using MoreInjuries.KnownDefs;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HearingLoss;

public class HearingLossExplosionsWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostPostApplyDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableBasicHearingDamage;

    public void PostPostApplyDamage(ref readonly DamageInfo dinfo)
    {
        Pawn patient = Target;
        if (dinfo.Def is not null && KnownDamageGroupNames.Explosions.Value.Contains(dinfo.Def.defName))
        {
            const float E_INVERSE = 1f / (float)Math.E;
            float chance = E_INVERSE * dinfo.Amount / ((E_INVERSE * dinfo.Amount) + 1);
            if (Rand.Chance(chance))
            {
                if (!patient.health.hediffSet.TryGetHediff(KnownHediffDefOf.HearingLossTemporary, out Hediff? hearingLoss))
                {
                    hearingLoss = HediffMaker.MakeHediff(KnownHediffDefOf.HearingLossTemporary, patient);
                    patient.health.AddHediff(hearingLoss);
                }
                // up to about +0.6 severity for very high damage, skewed towards higher values
                float baseSeverity = Rand.Range(0f, chance);
                hearingLoss.Severity = Mathf.Clamp01(hearingLoss.Severity + Mathf.Pow(baseSeverity, E_INVERSE));

                HearingLossHelper.TryMakePermanentIfApplicable(patient, hearingLoss);
            }
        }
    }
}
