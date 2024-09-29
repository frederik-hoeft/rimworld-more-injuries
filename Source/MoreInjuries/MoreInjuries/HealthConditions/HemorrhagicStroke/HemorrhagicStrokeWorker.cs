using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.HemorrhagicStroke;

public class HemorrhagicStrokeWorker(InjuryComp parent) : InjuryWorker(parent), IPostTakeDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableHemorrhagicStroke;

    public void PostTakeDamage(DamageWorker.DamageResult damage, ref readonly DamageInfo dinfo)
    {
        Pawn patient = Target;
        int totalBruises = 0;
        int severeBruises = 0;
        int legBruises = 0;

        foreach (Hediff hediff in patient.health.hediffSet.hediffs)
        {
            if (hediff.def == KnownHediffDefOf.Bruise)
            {
                totalBruises++;
                if (hediff.Severity >= 14)
                {
                    severeBruises++;
                }
                if (hediff.sourceBodyPartGroup == BodyPartGroupDefOf.Legs)
                {
                    legBruises++;
                }
            }
        }
        // TODO: add a setting for the chance
        // apply hemorrhagic stroke if any of the thresholds are met
        if ((totalBruises > 16 || severeBruises > 8 || legBruises > 4) && Rand.Chance(0.07f))
        {
            patient.health.AddHediff(KnownHediffDefOf.HemorrhagicStroke, patient.health.hediffSet.GetBrain());
        }
    }
}
