using MoreInjuries.KnownDefs;
using RimWorld;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HearingLoss;

public class HearingLossExplosionsWorker(InjuryComp parent) : InjuryWorker(parent), IPostPostApplyDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableBasicHearingDamage;

    public static Lazy<HashSet<string>> ExplosionDamageDefNames { get; } = new Lazy<HashSet<string>>(() => 
    [
        DamageDefOf.Bomb.defName,
        "Thermobaric"
    ], LazyThreadSafetyMode.None);

    public void PostPostApplyDamage(ref readonly DamageInfo dinfo)
    {
        Pawn patient = Target;
        if (dinfo.Def is not null && ExplosionDamageDefNames.Value.Contains(dinfo.Def.defName))
        {
            const float E_INVERSE = 1f / (float)Math.E;
            float chance = E_INVERSE * dinfo.Amount / ((E_INVERSE * dinfo.Amount) + 1);
            if (Rand.Chance(chance))
            {
                if (!patient.health.hediffSet.TryGetHediff(KnownHediffDefOf.HearingLoss, out Hediff? hearingLoss))
                {
                    hearingLoss = HediffMaker.MakeHediff(KnownHediffDefOf.HearingLoss, patient);
                    patient.health.AddHediff(hearingLoss);
                }
                // up to about +0.6 severity for very high damage, skewed towards higher values
                float baseSeverity = Rand.Range(0f, chance);
                hearingLoss.Severity = Mathf.Clamp01(hearingLoss.Severity + Mathf.Pow(baseSeverity, E_INVERSE));
            }
        }
    }
}
