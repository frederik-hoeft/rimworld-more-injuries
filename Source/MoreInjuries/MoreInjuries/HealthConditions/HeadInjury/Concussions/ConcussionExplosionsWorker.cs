using MoreInjuries.Defs.WellKnown;
using UnityEngine;
using Verse;

namespace MoreInjuries.HealthConditions.HeadInjury.Concussions;

internal sealed class ConcussionExplosionsWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostPostApplyDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableConcussion;

    public void PostPostApplyDamage(ref readonly DamageInfo dinfo)
    {
        Pawn patient = Pawn;
        if (dinfo.Def is not null && KnownDamageGroupNames.Explosions.Value.Contains(dinfo.Def.defName))
        {
            // ((1 / e) * x) / ((1 / e) * x + 1)
            const float E_INVERSE = 1f / (float)Math.E;
            float chance = E_INVERSE * dinfo.Amount / ((E_INVERSE * dinfo.Amount) + 1);
            if (Rand.Chance(chance * MoreInjuriesMod.Settings.ConcussionChance) && patient.health.hediffSet.GetBrain() is BodyPartRecord brain)
            {
                if (!patient.health.hediffSet.TryGetHediff(KnownHediffDefOf.Concussion, out Hediff? concussion))
                {
                    concussion = HediffMaker.MakeHediff(KnownHediffDefOf.Concussion, patient);
                    patient.health.AddHediff(concussion, brain);
                }
                // the base severity is a random value between 0 and the initial chance distribution
                // commonly between 0.6 and 0.9, possibly even higher for very high damage
                float baseSeverity = Rand.Range(0f, chance);
                // and now scale all of that logarithmically using
                // f(x)=1/(1+e^(10 * (0.4-x)))
                // such that at an inital chance of 0.8, there is a 50% chance of a severity of above and below 0.5
                // in cases of high damage, the severity will be skewed towards higher values
                float severity = 1f / (1f + Mathf.Exp(10f * (0.4f - baseSeverity)));
                // no clamping required, the function is already bounded between >0.01 and ~0.99
                concussion.Severity = severity;
            }
        }
    }
}