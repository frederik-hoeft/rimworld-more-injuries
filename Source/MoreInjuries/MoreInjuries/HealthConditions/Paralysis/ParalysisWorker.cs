using MoreInjuries.Defs.WellKnown;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.Paralysis;

internal sealed class ParalysisWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostTakeDamageHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableParalysis;

    public void PostTakeDamage(DamageWorker.DamageResult damage, ref readonly DamageInfo dinfo)
    {
        if (damage.parts is not List<BodyPartRecord> { Count: > 0 } bodyParts)
        {
            return;
        }
        Pawn patient = Target;
        if (bodyParts.FirstOrDefault(static bodyPart => bodyPart.def == KnownBodyPartDefOf.SpinalCord) is BodyPartRecord spinalCord && !patient.health.hediffSet.PartIsMissing(spinalCord))
        {
            // determine whether to apply paralysis based on the defined 50% damage threshold
            // (in 50% of all cases, the spinal cord will be damaged if the specified threshold damage is applied, scaling accordingly)
            float threshold = MoreInjuriesMod.Settings.ParalysisDamageTreshold50Percent;
            float actualDamage = damage.totalDamageDealt;
            float chance = actualDamage / threshold * 0.5f;
            if (Rand.Chance(chance))
            {
                Hediff paralysis = HediffMaker.MakeHediff(KnownHediffDefOf.SpinalCordParalysis, patient, spinalCord);
                float relativeDamage = actualDamage / patient.health.hediffSet.GetPartHealth(spinalCord);
                // severity of the paralysis is scaled based on the relative damage applied to the spinal cord
                paralysis.Severity = Rand.Range(0f, 2f * relativeDamage);
                patient.health.AddHediff(paralysis, spinalCord);
            }
        }
    }

    internal sealed class Factory : IInjuryWorkerFactory
    {
        public InjuryWorker Create(MoreInjuryComp parent) => new ParalysisWorker(parent);
    }
}
