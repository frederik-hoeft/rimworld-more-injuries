using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.HydrostaticShock;

// TODO: controversial
internal class HydrostaticShockWorker(InjuryComp parent) : InjuryWorker(parent), IPostTakeDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.UseHydrostaticShock;

    public void PostTakeDamage(DamageWorker.DamageResult damage, ref readonly DamageInfo dinfo)
    {
        Pawn patient = Target;
        if (!damage.diminished && damage.totalDamageDealt > 31 && dinfo.Def == DamageDefOf.Bullet)
        {
            // TODO: add a setting for the chance
            if (Rand.Chance(0.10f))
            {
                patient.health.AddHediff(HediffMaker.MakeHediff(KnownHediffDefOf.HemorrhagicStroke, patient));
            }
        }
    }
}
