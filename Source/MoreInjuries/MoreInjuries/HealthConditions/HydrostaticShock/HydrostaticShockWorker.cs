using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.HydrostaticShock;

internal class HydrostaticShockWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostTakeDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableHydrostaticShock;

    public void PostTakeDamage(DamageWorker.DamageResult damage, ref readonly DamageInfo dinfo)
    {
        Pawn patient = Target;
        if (!damage.diminished && damage.totalDamageDealt > 31f && dinfo.Def == DamageDefOf.Bullet)
        {
            if (Rand.Chance(MoreInjuriesMod.Settings.HydrostaticShockChanceOnDamage))
            {
                patient.health.AddHediff(HediffMaker.MakeHediff(KnownHediffDefOf.HemorrhagicStroke, patient));
            }
        }
    }
}
