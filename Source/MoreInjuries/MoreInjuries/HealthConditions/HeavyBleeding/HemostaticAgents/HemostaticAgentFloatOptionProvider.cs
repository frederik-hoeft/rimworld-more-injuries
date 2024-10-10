using MoreInjuries.KnownDefs;
using MoreInjuries.Things;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.HemostaticAgents;

internal class HemostaticAgentFloatOptionProvider(InjuryWorker parent) : ICompFloatMenuOptionsHandler
{
    public bool IsEnabled => true;

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        Pawn patient = parent.Target;
        if (!builder.Keys.Contains(UITreatmentOption.UseHemostaticAgent) && patient.health.hediffSet.hediffs.Any(JobDriver_HemostasisBase.JobCanTreat))
        {
            builder.Keys.Add(UITreatmentOption.UseHemostaticAgent);
            if (MedicalDeviceHelper.GetReasonForDisabledProcedure(selectedPawn, patient, JobDriver_UseHemostaticAgent.JOB_LABEL) is string failure)
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, KnownThingDefOf.HemostaticAgent, JobDriver_HemostasisBase.JobCanTreat, fromInventoryOnly: true) is Thing inventoryThing)
            {
                builder.Options.Add(new FloatMenuOption($"{JobDriver_UseHemostaticAgent.JOB_LABEL} (from inventory)", JobDriver_UseHemostaticAgent.GetDispatcher(selectedPawn, patient, inventoryThing, fromInventoryOnly: true).StartJob));
            }
            else if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, KnownThingDefOf.HemostaticAgent, JobDriver_HemostasisBase.JobCanTreat) is not Thing thing)
            {
                builder.Options.Add(new FloatMenuOption($"{JobDriver_UseHemostaticAgent.JOB_LABEL}: no hemostatic agents available", null));
            }
            else
            {
                builder.Options.Add(new FloatMenuOption(JobDriver_UseHemostaticAgent.JOB_LABEL, JobDriver_UseHemostaticAgent.GetDispatcher(selectedPawn, patient, thing, fromInventoryOnly: false).StartJob));
            }
        }
    }
}
