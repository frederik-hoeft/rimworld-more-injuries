using MoreInjuries.Debug;
using MoreInjuries.Defs.WellKnown;
using MoreInjuries.HealthConditions.CardiacArrest;
using MoreInjuries.HealthConditions.Secondary;
using MoreInjuries.Things;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.Choking;

internal sealed class ChokingWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostApplyDamageToPartHandler, ICompFloatMenuOptionsHandler
{
    private static readonly Dictionary<BodyPartDef, bool> s_bodyPartAffectsBreathingCache = [];

    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableChoking;

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        Pawn patient = Pawn;
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

    public void ApplyDamageToPart(ref readonly DamageInfo dinfo, Pawn pawn, DamageWorker.DamageResult result)
    {
        Pawn patient = Pawn;
        if (dinfo.HitPart is not { } bodyPart || !AffectsBreathing(bodyPart) || result.hediffs is not { Count: > 0 })
        {
            return;
        }
        // there was damage done to a body part essential for breathing. Check for any bleeding injuries on the respiratory system.
        foreach (Hediff hediff in result.hediffs)
        {
            if (hediff is Hediff_Injury { Bleeding: true, Part: { } part } injury
                && part == bodyPart
                && injury.BleedRate >= MoreInjuriesMod.Settings.ChokingMinimumBleedRate
                && Rand.Chance(MoreInjuriesMod.Settings.ChokingChanceOnDamage))
            {
                Hediff choking = HediffMaker.MakeHediff(KnownHediffDefOf.ChokingOnBlood, patient);
                if (!choking.TryGetComp(out HediffComp_Choking? chokingComp))
                {
                    Logger.Error($"Failed to get ChokingHediffComp from choking hediff! Could not apply choking condition to {patient}. This is a bug.");
                    return;
                }
                DebugAssert.IsNotNull(chokingComp);
                chokingComp.SetSource(injury);
                if (choking.TryGetComp(out HediffComp_CausedBy? causedBy))
                {
                    DebugAssert.IsNotNull(causedBy);
                    causedBy.AddCause(injury);
                }
                patient.health.AddHediff(choking);
            }
        }
    }

    private static bool AffectsBreathing(BodyPartRecord bodyPart)
    {
        BodyPartDef bodyPartDef = bodyPart.def;
        if (!s_bodyPartAffectsBreathingCache.TryGetValue(bodyPartDef, out bool value))
        {
            foreach (BodyPartTagDef tag in bodyPartDef.tags)
            {
                if (tag == BodyPartTagDefOf.BreathingSource || tag == BodyPartTagDefOf.BreathingPathway)
                {
                    value = true;
                    break;
                }
            }
            s_bodyPartAffectsBreathingCache[bodyPartDef] = value;
        }
        return value;
    }
}