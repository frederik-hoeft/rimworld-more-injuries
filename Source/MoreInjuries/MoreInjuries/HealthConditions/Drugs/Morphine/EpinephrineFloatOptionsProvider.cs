using MoreInjuries.AI;
using MoreInjuries.Defs.WellKnown;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs.Morphine;

public class MorphineFloatOptionsProvider(InjuryWorker parent) : DrugFloatOptionsProvider(parent)
{
    public override bool IsEnabled => KnownResearchProjectDefOf.MorphineSynthesis.IsFinished;

    protected override UITreatmentOption UITreatmentOption => UITreatmentOption.UseMorphine;

    protected override string JobLabelKey => JobDriver_UseMorphine.JOB_LABEL_KEY;

    protected override ThingDef DrugThingDef => KnownThingDefOf.Morphine;

    protected override IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device) =>
        JobDriver_UseMorphine.GetDispatcher(doctor, patient, device);
}