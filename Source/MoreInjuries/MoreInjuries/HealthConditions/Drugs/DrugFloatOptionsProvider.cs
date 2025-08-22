using MoreInjuries.AI.Jobs;
using MoreInjuries.Localization;
using MoreInjuries.Things;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs;

public abstract class DrugFloatOptionsProvider(InjuryWorker parent) : ICompFloatMenuOptionsHandler
{
    public abstract bool IsEnabled { get; }

    protected abstract UITreatmentOption UITreatmentOption { get; }

    protected virtual bool RequiresTreatment(Pawn patient) => true;

    protected abstract string JobLabelKey { get; }

    protected abstract ThingDef DrugThingDef { get; }

    protected abstract IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device);

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        Pawn patient = parent.Pawn;
        if (!builder.Keys.Contains(UITreatmentOption) && RequiresTreatment(patient))
        {
            builder.Keys.Add(UITreatmentOption);

            if (MedicalDeviceHelper.FindMedicalDevice(selectedPawn, patient, DrugThingDef) is not Thing injector)
            {
                return;
            }
            if (MedicalDeviceHelper.GetCauseForDisabledProcedure(selectedPawn, patient, JobLabelKey) is { FailureReason: string failure })
            {
                builder.Options.Add(new FloatMenuOption(failure, null));
                return;
            }
            TaggedString label = selectedPawn.inventory.Contains(injector)
                ? Named.Keys.Procedure_FromInventory.Translate(JobLabelKey.Translate())
                : JobLabelKey.Translate();

            builder.Options.Add(new FloatMenuOption(label, GetDispatcher(selectedPawn, patient, injector).StartJob));
        }
    }
}