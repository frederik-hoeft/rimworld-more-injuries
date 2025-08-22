using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Extensions;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.HydrostaticShock;

internal sealed class HydrostaticShockWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostTakeDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableHydrostaticShock;

    public void PostTakeDamage(DamageWorker.DamageResult damage, ref readonly DamageInfo dinfo)
    {
        Pawn patient = Pawn;
        if (damage is { diminished: false, totalDamageDealt: > 31f } 
            && dinfo.Def == DamageDefOf.Bullet 
            && Rand.Chance(MoreInjuriesMod.Settings.HydrostaticShockChanceOnDamage) 
            && patient.health.hediffSet.GetBrain() is BodyPartRecord brain)
        {
            if (!patient.health.hediffSet.TryGetFirstHediffMatchingPart(brain, KnownHediffDefOf.HemorrhagicStroke, out Hediff? trauma))
            {
                trauma = HediffMaker.MakeHediff(KnownHediffDefOf.HemorrhagicStroke, patient);
                patient.health.AddHediff(trauma, brain);
            }
            trauma.Severity += 0.1f;
        }
    }
}