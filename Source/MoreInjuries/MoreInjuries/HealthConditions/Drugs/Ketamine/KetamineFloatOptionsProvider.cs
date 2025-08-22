using MoreInjuries.AI.Jobs;
using MoreInjuries.Defs.WellKnown;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs.Ketamine;

public class KetamineFloatOptionsProvider(InjuryWorker parent) : DrugFloatOptionsProvider(parent)
{
    public override bool IsEnabled => KnownResearchProjectDefOf.KetamineSynthesis.IsFinished;

    protected override UITreatmentOption UITreatmentOption => UITreatmentOption.UseKetamine;

    protected override string JobLabelKey => JobDriver_UseKetamine.JOB_LABEL_KEY;

    protected override ThingDef DrugThingDef => KnownThingDefOf.Ketamine;

    protected override IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device) =>
        JobDriver_UseKetamine.GetDispatcher(doctor, patient, device);
}
