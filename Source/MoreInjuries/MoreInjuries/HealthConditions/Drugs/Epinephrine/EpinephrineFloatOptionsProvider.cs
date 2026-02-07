using MoreInjuries.AI.Jobs;
using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Extensions;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs.Epinephrine;

internal sealed class EpinephrineFloatOptionsProvider(InjuryWorker parent) : DrugFloatOptionsProvider(parent)
{
    public override bool IsEnabled => MoreInjuriesMod.Settings.EnableAdrenaline && KnownResearchProjectDefOf.EpinephrineSynthesis.IsFinished;

    protected override UITreatmentOption UITreatmentOption => UITreatmentOption.UseEpinephrine;

    protected override string JobLabelKey => JobDriver_UseEpinephrine.JOB_LABEL_KEY;

    protected override ThingDef DrugThingDef => KnownThingDefOf.Epinephrine;

    protected override bool CanTreat(Pawn patient, Pawn doctor) => !patient.IsActivelyHostileTo(doctor);

    protected override IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device) =>
        JobDriver_UseEpinephrine.GetDispatcher(doctor, patient, device);
}
