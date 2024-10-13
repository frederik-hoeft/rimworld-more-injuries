using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.Injectors;

public abstract class JobDriver_UseInjector_GivesHediff : JobDriver_UseInjector
{
    protected abstract HediffDef HediffDef { get; }

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        if (device?.def.GetModExtension<InjectorProps_ModExtension>() is not InjectorProps_ModExtension extension)
        {
            Logger.Warning($"{nameof(JobDriver_UseInjector_GivesHediff)} failed to apply {HediffDef} because the device is null or has no {nameof(InjectorProps_ModExtension)}");
            EndJobWith(JobCondition.Incompletable);
            return;
        }
        if (device.Destroyed)
        {
            Logger.Error($"{nameof(JobDriver_UseInjector_GivesHediff)} failed to apply {HediffDef} because the device was destroyed. What's going on?");
            EndJobWith(JobCondition.Incompletable);
            return;
        }
        device.DecreaseStack();
        Hediff? hediff = patient.health.hediffSet.GetFirstHediffOfDef(HediffDef);
        if (hediff is null)
        {
            hediff = HediffMaker.MakeHediff(HediffDef, patient);
            patient.health.AddHediff(hediff);
        }
        float severity = hediff.Severity + extension.SeverityOffset;
        if (severity <= 0f)
        {
            patient.health.RemoveHediff(hediff);
            return;
        }
        hediff.Severity = Mathf.Min(severity, HediffDef.maxSeverity);
    }
}
