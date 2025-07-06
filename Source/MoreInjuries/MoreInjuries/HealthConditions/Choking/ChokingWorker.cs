﻿using MoreInjuries.Defs.WellKnown;
using MoreInjuries.HealthConditions.CardiacArrest;
using MoreInjuries.HealthConditions.Secondary;
using MoreInjuries.Things;
using RimWorld;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.Choking;

internal sealed class ChokingWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostPostApplyDamageHandler, ICompFloatMenuOptionsHandler
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableChoking;

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        Pawn patient = Target;
        if (selectedPawn == patient || !selectedPawn.Drafted)
        {
            return;
        }
        // only show options if they are drafted
        if (!builder.Keys.Contains(UITreatmentOption.PerformCpr) && patient.health.hediffSet.hediffs.Any(static hediff => Array.IndexOf(JobDriver_PerformCpr.TargetHediffDefs, hediff.def) != -1))
        {
            builder.Keys.Add(UITreatmentOption.PerformCpr);
            if (!KnownResearchProjectDefOf.Cpr.IsFinished)
            {
                return;
            }
            if (MedicalDeviceHelper.GetCauseForDisabledProcedure(selectedPawn, patient, JobDriver_PerformCpr.JOB_LABEL_KEY) is { FailureReason: string failure })
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
            }
            else
            {
                builder.Options.Add(new FloatMenuOption(JobDriver_PerformCpr.JOB_LABEL_KEY.Translate(), JobDriver_PerformCpr.GetDispatcher(selectedPawn, patient).StartJob));
            }
        }
        if (!builder.Keys.Contains(UITreatmentOption.UseSuctionDevice) && patient.health.hediffSet.hediffs.Any(static hediff => Array.IndexOf(JobDriver_UseSuctionDevice.TargetHediffDefs, hediff.def) != -1))
        {
            builder.Keys.Add(UITreatmentOption.UseSuctionDevice);
            if (!KnownResearchProjectDefOf.EmergencyMedicine.IsFinished)
            {
                return;
            }
            if (MedicalDeviceHelper.GetCauseForDisabledProcedure(selectedPawn, patient, JobDriver_UseSuctionDevice.JOB_LABEL_KEY) is { FailureReason: string failure })
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, KnownThingDefOf.SuctionDevice, JobDriver_UseSuctionDevice.TargetHediffDefs) is not Thing suctionDevice)
            {
                builder.Options.Add(new FloatMenuOption("MI_UseSuctionDeviceFailed_Unavailable".Translate(JobDriver_UseSuctionDevice.JOB_LABEL_KEY.Translate()), null));
            }
            else
            {
                builder.Options.Add(new FloatMenuOption(JobDriver_UseSuctionDevice.JOB_LABEL_KEY.Translate(), JobDriver_UseSuctionDevice.GetDispatcher(selectedPawn, patient, suctionDevice).StartJob));
            }
        }
    }

    public void PostPostApplyDamage(ref readonly DamageInfo dinfo)
    {
        Pawn patient = Target;
        foreach (Hediff_Injury injury in patient.health.hediffSet.GetHediffsTendable().OfType<Hediff_Injury>())
        {
            if (injury is { Bleeding: true } && injury.BleedRate >= MoreInjuriesMod.Settings.ChokingMinimumBleedRate
                && injury.Part.def.tags.Any(static tag => tag == BodyPartTagDefOf.BreathingSource || tag == BodyPartTagDefOf.BreathingPathway)
                && Rand.Chance(MoreInjuriesMod.Settings.ChokingChanceOnDamage))
            {
                Hediff choking = HediffMaker.MakeHediff(KnownHediffDefOf.ChokingOnBlood, patient);
                if (!choking.TryGetComp(out HediffComp_Choking? chokingComp))
                {
                    Logger.Error($"Failed to get ChokingHediffComp from choking hediff! Could not apply choking condition to {patient}. This is a bug.");
                    return;
                }
                chokingComp!.Source = injury;
                if (choking.TryGetComp(out HediffComp_CausedBy? causedBy))
                {
                    causedBy!.AddCause(injury);
                }
                patient.health.AddHediff(choking);
            }
        }
    }

    public sealed class Factory : IInjuryWorkerFactory
    {
        public InjuryWorker Create(MoreInjuryComp parent) => new ChokingWorker(parent);
    }
}
