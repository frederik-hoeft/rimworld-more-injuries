using MoreInjuries.KnownDefs;
using Verse;

namespace MoreInjuries.HealthConditions.AdrenalineRush;

internal class AdrenalineWorker(InjuryComp parent) : InjuryWorker(parent), IPostTakeDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableAdrenaline;

    public void PostTakeDamage(DamageWorker.DamageResult damage, ref readonly DamageInfo dinfo)
    {
        Pawn patient = Target;
        // TODO: is totalDamageDealt even capped at 1.0f?
        if (Rand.Chance(damage.totalDamageDealt))
        {
            if (!patient.health.hediffSet.TryGetHediff(KnownHediffDefOf.AdrenalineRush, out Hediff? adrenalineDump))
            {
                // add new hediff
                adrenalineDump = HediffMaker.MakeHediff(KnownHediffDefOf.AdrenalineRush, patient);
                adrenalineDump.Severity = 0;
                patient.health.AddHediff(adrenalineDump);
            }
            // TODO: negative severity?
            float severity = Rand.Range(damage.totalDamageDealt * -10f, damage.totalDamageDealt * 2);
            adrenalineDump.Severity += severity;
        }
    }
}
