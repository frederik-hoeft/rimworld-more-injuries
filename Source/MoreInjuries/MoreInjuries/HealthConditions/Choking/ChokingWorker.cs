using MoreInjuries.KnownDefs;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.Choking;

internal class ChokingWorker(InjuryComp parent) : InjuryWorker(parent), IPostPostApplyDamageHandler, ICompFloatMenuOptionsHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.ChokingEnabled;

    public IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selectedPawn)
    {
        Pawn patient = Target;
        // can't perform CPR on yourself, though that would be pretty wild
        if (!ReferenceEquals(selectedPawn, patient))
        {
            if (patient.health.hediffSet.hediffs.Any(PerformCprJob.CanBeTreatedWithCpr))
            {
                return 
                [
                    new FloatMenuOption("Perform CPR", () => selectedPawn.jobs.StartJob(new Job(def: KnownJobDefOf.PerformCpr, targetA: patient), JobCondition.InterruptForced))
                ];
            }
        }
        return [];
    }

    public void PostPostApplyDamage(ref readonly DamageInfo dinfo)
    {
        Pawn patient = Target;
        foreach (Hediff_Injury injury in patient.health.hediffSet.GetHediffsTendable().OfType<Hediff_Injury>())
        {
            if (injury is { Bleeding: true, BleedRate: >= 0.20f }
                && injury.Part.def.tags.Any(tag => tag == BodyPartTagDefOf.BreathingSource || tag == BodyPartTagDefOf.BreathingPathway)
                && Rand.Chance(0.70f))
            {
                Hediff choking = HediffMaker.MakeHediff(KnownHediffDefOf.ChokingOnBlood, patient);
                if (choking.TryGetComp(out ChokingHediffComp? comp))
                {
                    comp!.Source = injury;
                    patient.health.AddHediff(choking);
                    return;
                }
                Log.Error("Failed to get ChokingHediffComp from choking hediff");
            }
        }
    }
}
