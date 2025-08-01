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
            float upperbound = Math.Min(damage.totalDamageDealt / (2f * damageThreshold), 0.75f);
            float severity = Rand.Range(0, upperbound);
            float newSeverity = adrenalineRush.Severity + severity;
            // We don't want pawns to constantly overdose on adrenaline, so we clamp the severity here. Max overdosing severity is 5.0, btw
            // starting from 1.75, overdosing will start to kick in. We allow slight overdosing to account for cases from medicine where
            // extreme pain and stress actually causes overdosing symptoms.
            // but we don't want to allow doses equivalent to multiple epinephrine injections, so we cap it at 1.8
            adrenalineRush.Severity = Mathf.Min(newSeverity, 1.8f);
        }
    }
}