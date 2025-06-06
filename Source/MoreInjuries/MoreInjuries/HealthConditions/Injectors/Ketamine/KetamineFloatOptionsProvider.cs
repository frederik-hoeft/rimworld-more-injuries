using MoreInjuries.AI;
using MoreInjuries.KnownDefs;
using Verse;

namespace MoreInjuries.HealthConditions.Injectors.Ketamine;

public class KetamineFloatOptionsProvider(InjuryWorker parent) : InjectorFloatOptionsProvider(parent)
{
    public override bool IsEnabled => KnownResearchProjectDefOf.KetamineSynthesis.IsFinished;

    protected override UITreatmentOption UITreatmentOption => UITreatmentOption.UseKetamine;

    protected override string JobLabelKey => JobDriver_UseKetamine.JOB_LABEL_KEY;

    protected override ThingDef InjectorDef => KnownThingDefOf.Ketamine;

    protected override IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device) =>
        JobDriver_UseKetamine.GetDispatcher(doctor, patient, device);
}
