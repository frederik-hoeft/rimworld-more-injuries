using MoreInjuries.AI.Jobs;
using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Extensions;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs.Morphine;

internal sealed class MorphineFloatOptionsProvider(InjuryWorker parent) : DrugFloatOptionsProvider(parent)
{
    public override bool IsEnabled => KnownResearchProjectDefOf.MorphineSynthesis.IsFinished;

    protected override UITreatmentOption UITreatmentOption => UITreatmentOption.UseMorphine;

    protected override string JobLabelKey => JobDriver_UseMorphine.JOB_LABEL_KEY;

    protected override ThingDef DrugThingDef => KnownThingDefOf.Morphine;

    protected override bool CanTreat(Pawn patient, Pawn doctor) => !patient.IsActivelyHostileTo(doctor);

    protected override IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device) =>
        JobDriver_UseMorphine.GetDispatcher(doctor, patient, device);
}