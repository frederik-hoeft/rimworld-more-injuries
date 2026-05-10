using MoreInjuries.AI.Jobs;
using MoreInjuries.Defs.WellKnown;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs.Chloroform;

internal sealed class ChloroformFloatOptionsProvider(InjuryWorker parent) : DrugFloatOptionsProvider(parent)
{
    public override bool IsEnabled => KnownResearchProjectDefOf.ChloroformSynthesis.IsFinished;

    protected override UITreatmentOption UITreatmentOption => UITreatmentOption.UseChloroform;

    protected override string JobLabelKey => JobDriver_UseChloroform.JOB_LABEL_KEY;

    protected override ThingDef DrugThingDef => KnownThingDefOf.Chloroform;

    protected override bool CanTreat(Pawn patient, Pawn doctor) => true;

    protected override IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device) =>
        JobDriver_UseChloroform.GetDispatcher(doctor, patient, device);
}
