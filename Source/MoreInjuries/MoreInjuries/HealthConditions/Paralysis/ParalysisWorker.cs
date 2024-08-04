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
            Hediff paralysis = HediffMaker.MakeHediff(KnownHediffDefOf.SpinalCordParalysis, patient, spinalCord);
            patient.health.AddHediff(paralysis, spinalCord);
        }
    }
}
