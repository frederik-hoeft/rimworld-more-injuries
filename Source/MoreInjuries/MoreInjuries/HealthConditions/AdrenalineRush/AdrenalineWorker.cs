using MoreInjuries.KnownDefs;
using Verse;

namespace MoreInjuries.HealthConditions.AdrenalineRush;

internal class AdrenalineWorker(InjuryComp parent) : InjuryWorker(parent), IPostTakeDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableAdrenaline;

    public void PostTakeDamage(DamageWorker.DamageResult damage, ref readonly DamageInfo dinfo)
    {
        Pawn patient = Target;
        if (Rand.Chance(MoreInjuriesMod.Settings.AdrenalineChanceOnDamage))
        {
            if (!patient.health.hediffSet.TryGetHediff(KnownHediffDefOf.AdrenalineRush, out Hediff? adrenalineDump))
            {
                // add new hediff
                adrenalineDump = HediffMaker.MakeHediff(KnownHediffDefOf.AdrenalineRush, patient);
                adrenalineDump.Severity = 0;
                patient.health.AddHediff(adrenalineDump);
            }
            // possible upperbound of the severity increases with the total damage received, but is capped at 0.5
            // 20 damage is a lot, so we use that as the upper threshold
            float upperbound = Math.Min(damage.totalDamageDealt / 20f * 0.5f, 0.5f);
            float severity = Rand.Range(0, upperbound);
            adrenalineDump.Severity += severity;
        }
    }
}
