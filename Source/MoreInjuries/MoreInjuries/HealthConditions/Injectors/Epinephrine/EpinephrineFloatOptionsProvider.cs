using MoreInjuries.AI;
using MoreInjuries.KnownDefs;
using Verse;

namespace MoreInjuries.HealthConditions.Injectors.Epinephrine;

public class EpinephrineFloatOptionsProvider(InjuryWorker parent) : InjectorFloatOptionsProvider(parent)
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableAdrenaline;

    protected override UITreatmentOption UITreatmentOption => UITreatmentOption.UseEpinephrine;

    protected override string JobLabel => JobDriver_UseEpinephrine.JOB_LABEL;

    protected override ThingDef InjectorDef => KnownThingDefOf.Epinephrine;

    protected override IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device) =>
        JobDriver_UseEpinephrine.GetDispatcher(doctor, patient, device);
}
