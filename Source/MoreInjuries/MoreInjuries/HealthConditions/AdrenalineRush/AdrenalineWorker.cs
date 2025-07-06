using MoreInjuries.Defs.WellKnown;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.AdrenalineRush;

internal sealed class AdrenalineWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostTakeDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableAdrenaline && !Target.IsShambler;

    public void PostTakeDamage(DamageWorker.DamageResult damage, ref readonly DamageInfo dinfo)
    {
        Pawn patient = Target;
        // clamp the damage threshold to 1 to avoid division by zero
        float damageThreshold = Mathf.Max(MoreInjuriesMod.Settings.CertainAdrenalineThreshold, 1f);
        if (Rand.Chance(MoreInjuriesMod.Settings.AdrenalineChanceOnDamage) || damage.totalDamageDealt > damageThreshold)
        {
            if (!patient.health.hediffSet.TryGetHediff(KnownHediffDefOf.AdrenalineRush, out Hediff? adrenalineRush))
            {
                // add new hediff
                adrenalineRush = HediffMaker.MakeHediff(KnownHediffDefOf.AdrenalineRush, patient);
                adrenalineRush.Severity = 0;
                patient.health.AddHediff(adrenalineRush);
            }
            // possible upperbound of the severity increases with the total damage received, but is capped at 0.75
            // 20 damage is a lot, so we use that as the upper threshold
            float upperbound = Math.Min(damage.totalDamageDealt / (2f * damageThreshold), 0.75f);
            float severity = Rand.Range(0, upperbound);
            adrenalineRush.Severity += severity;
        }
    }

    public sealed class Factory : IInjuryWorkerFactory
    {
        public InjuryWorker Create(MoreInjuryComp parent) => new AdrenalineWorker(parent);
    }
}
