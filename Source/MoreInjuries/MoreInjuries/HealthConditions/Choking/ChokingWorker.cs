using MoreInjuries.HealthConditions.CardiacArrest;
using MoreInjuries.KnownDefs;
using MoreInjuries.Things;
using RimWorld;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.Choking;

internal class ChokingWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostPostApplyDamageHandler, ICompFloatMenuOptionsHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableChoking;

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        Pawn patient = Target;
        if (selectedPawn == patient)
        {
            return;
        }
        if (!builder.Keys.Contains(UITreatmentOption.PerformCpr) && patient.health.hediffSet.hediffs.Any(hediff => Array.IndexOf(JobDriver_PerformCpr.TargetHediffDefs, hediff.def) != -1))
        {
            builder.Keys.Add(UITreatmentOption.PerformCpr);
            if (MedicalDeviceHelper.GetReasonForDisabledProcedure(selectedPawn, patient, JobDriver_PerformCpr.JOB_LABEL) is string failure)
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
            }
            else
            {
                builder.Options.Add(new FloatMenuOption(JobDriver_PerformCpr.JOB_LABEL, JobDriver_PerformCpr.GetDispatcher(selectedPawn, patient).StartJob));
            }
        }
        if (!builder.Keys.Contains(UITreatmentOption.UseSuctionDevice) && patient.health.hediffSet.hediffs.Any(hediff => Array.IndexOf(JobDriver_UseSuctionDevice.TargetHediffDefs, hediff.def) != -1))
        {
            builder.Keys.Add(UITreatmentOption.UseSuctionDevice);
            if (MedicalDeviceHelper.GetReasonForDisabledProcedure(selectedPawn, patient, JobDriver_UseSuctionDevice.JOB_LABEL) is string failure)
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, KnownThingDefOf.SuctionDevice, JobDriver_UseSuctionDevice.TargetHediffDefs) is not Thing suctionDevice)
            {
                builder.Options.Add(new FloatMenuOption($"{JobDriver_UseSuctionDevice.JOB_LABEL}: no suction device available", null));
            }
            else
            {
                builder.Options.Add(new FloatMenuOption(JobDriver_UseSuctionDevice.JOB_LABEL, JobDriver_UseSuctionDevice.GetDispatcher(selectedPawn, patient, suctionDevice).StartJob));
            }
        }
    }

    public void PostPostApplyDamage(ref readonly DamageInfo dinfo)
    {
        Pawn patient = Target;
        foreach (Hediff_Injury injury in patient.health.hediffSet.GetHediffsTendable().OfType<Hediff_Injury>())
        {
            if (injury is { Bleeding: true } && injury.BleedRate >= MoreInjuriesMod.Settings.ChokingMinimumBleedRate
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
