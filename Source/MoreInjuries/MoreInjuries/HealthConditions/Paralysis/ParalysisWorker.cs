using MoreInjuries.KnownDefs;
using Verse;

namespace MoreInjuries.HealthConditions.Paralysis;

internal class ParalysisWorker(InjuryComp parent) : InjuryWorker(parent), IPostTakeDamageHandler
{
    public override bool IsEnabled => true;

    public void PostTakeDamage(DamageWorker.DamageResult damage, ref readonly DamageInfo dinfo)
    {
        if (damage.parts is null)
        {
            return;
        }
        Pawn patient = Target;
        if (damage.parts.FirstOrDefault(bodyPart => bodyPart.def == KnownBodyPartDefOf.SpinalCord) is BodyPartRecord spinalCord)
        {
            // determine whether to apply paralysis based on the defined 50% damage threshold
            // (in 50% of all cases, the spinal cord will be damaged if the specified threshold damage is applied, scaling accordingly)
            float threshold = MoreInjuriesMod.Settings.ParalysisDamageTreshold50Percent;
            float actualDamage = damage.totalDamageDealt;
            float chance = actualDamage / threshold * 0.5f;
            if (Rand.Chance(chance))
            {
                Hediff paralysis = HediffMaker.MakeHediff(KnownHediffDefOf.SpinalCordParalysis, patient, spinalCord);
                patient.health.AddHediff(paralysis, spinalCord);
            }
        }
    }
}
