using MoreInjuries.KnownDefs;
using MoreInjuries.Things;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.Choking;

internal class ChokingWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostPostApplyDamageHandler, ICompFloatMenuOptionsHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableChoking;

    public IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selectedPawn)
    {
        Pawn patient = Target;
        // can never self-treat choking or cardiac arrest
        if (!ReferenceEquals(selectedPawn, patient))
        {
            foreach (FloatMenuOption option in CreatePerformCprOptions(selectedPawn, patient))
            {
                yield return option;
            }
            foreach (FloatMenuOption option in CreateClearAirwaysOptions(selectedPawn, patient))
            {
                yield return option;
            }
        }
    }

    private static FloatMenuOption[] CreateClearAirwaysOptions(Pawn doctor, Pawn patient)
    {
        if (!patient.health.hediffSet.hediffs.Any(hediff => Array.IndexOf(JobDriver_ClearAirways.TargetHediffDefs, hediff.def) != -1))
        {
            return [];
        }
        string? failure = MedicalDeviceHelper.GetReasonForDisabledProcedure(doctor, patient, "Clear airways");
        if (failure is not null)
        {
            return [new FloatMenuOption(failure, null)];
        }
        Thing? device = MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.SuctionDevice, JobDriver_ClearAirways.TargetHediffDefs);
        if (device is null)
        {
            return [new FloatMenuOption("Clear airways: no suction device available", null)];
        }
        void startClearingAirways()
        {
            Job job = JobMaker.MakeJob(KnownJobDefOf.ApplySplintJob, patient, device);
            job.count = 1;
            doctor.jobs.TryTakeOrderedJob(job);
        }
        return [new FloatMenuOption("Clear airways", startClearingAirways)];
    }

    private static FloatMenuOption[] CreatePerformCprOptions(Pawn doctor, Pawn patient)
    {
        if (!patient.health.hediffSet.hediffs.Any(hediff => Array.IndexOf(JobDriver_PerformCpr.TargetHediffDefs, hediff.def) != -1))
        {
            return [];
        }
        string? failure = MedicalDeviceHelper.GetReasonForDisabledProcedure(doctor, patient, "Perform CPR");
        if (failure is not null)
        {
            return [new FloatMenuOption(failure, null)];
        }
        void startCpr()
        {
            Job job = JobMaker.MakeJob(KnownJobDefOf.PerformCprJob, patient);
            job.count = 1;
            doctor.jobs.TryTakeOrderedJob(job);
        }
        return [new FloatMenuOption("Perform CPR", startCpr)];
    }

    public void PostPostApplyDamage(ref readonly DamageInfo dinfo)
    {
        Pawn patient = Target;
        foreach (Hediff_Injury injury in patient.health.hediffSet.GetHediffsTendable().OfType<Hediff_Injury>())
        {
            if (injury is { Bleeding: true, BleedRate: >= 0.20f }
                && injury.Part.def.tags.Any(tag => tag == BodyPartTagDefOf.BreathingSource || tag == BodyPartTagDefOf.BreathingPathway)
                && Rand.Chance(MoreInjuriesMod.Settings.ChokingChanceOnDamage))
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
