using MoreInjuries.AI.Jobs;
using MoreInjuries.Defs.WellKnown;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs.Chloroform;

public class ChloroformFloatOptionsProvider(InjuryWorker parent) : DrugFloatOptionsProvider(parent)
{
    public override bool IsEnabled => KnownResearchProjectDefOf.ChloroformSynthesis.IsFinished;

    protected override UITreatmentOption UITreatmentOption => UITreatmentOption.UseChloroform;

    protected override string JobLabelKey => JobDriver_UseChloroform.JOB_LABEL_KEY;

    protected override ThingDef DrugThingDef => KnownThingDefOf.Chloroform;

    protected override IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device) =>
        JobDriver_UseChloroform.GetDispatcher(doctor, patient, device);
}
