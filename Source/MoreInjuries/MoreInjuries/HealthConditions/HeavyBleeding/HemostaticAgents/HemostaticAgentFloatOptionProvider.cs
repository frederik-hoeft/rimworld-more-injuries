using MoreInjuries.KnownDefs;
using MoreInjuries.Localization;
using MoreInjuries.Things;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.HemostaticAgents;

internal class HemostaticAgentFloatOptionProvider(InjuryWorker parent) : ICompFloatMenuOptionsHandler
{
    public bool IsEnabled => true;

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        Pawn patient = parent.Target;
        if (!builder.Keys.Contains(UITreatmentOption.UseHemostaticAgent) && selectedPawn.Drafted && patient.health.hediffSet.hediffs.Any(JobDriver_HemostasisBase.JobCanTreat))
        {
            builder.Keys.Add(UITreatmentOption.UseHemostaticAgent);
            if (MedicalDeviceHelper.GetCauseForDisabledProcedure(selectedPawn, patient, JobDriver_UseHemostaticAgent.JOB_LABEL_KEY) is { FailureReason: string failure })
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, KnownThingDefOf.HemostaticAgent, JobDriver_HemostasisBase.JobCanTreat, fromInventoryOnly: true) is Thing inventoryThing)
            {
                builder.Options.Add(new FloatMenuOption(Named.Keys.Procedure_FromInventory.Translate(JobDriver_UseHemostaticAgent.JOB_LABEL_KEY.Translate()), JobDriver_UseHemostaticAgent.GetDispatcher(selectedPawn, patient, inventoryThing, fromInventoryOnly: true).StartJob));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, KnownThingDefOf.HemostaticAgent, JobDriver_HemostasisBase.JobCanTreat) is not Thing thing)
            {
                builder.Options.Add(new FloatMenuOption("MI_UseHemostaticAgentFailed_Unavailable".Translate(JobDriver_UseHemostaticAgent.JOB_LABEL_KEY.Translate()), null));
            }
            else
            {
                builder.Options.Add(new FloatMenuOption(JobDriver_UseHemostaticAgent.JOB_LABEL_KEY.Translate(), JobDriver_UseHemostaticAgent.GetDispatcher(selectedPawn, patient, thing, fromInventoryOnly: false).StartJob));
            }
        }
    }
}
