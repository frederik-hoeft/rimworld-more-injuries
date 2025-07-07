using MoreInjuries.AI.Jobs;
using MoreInjuries.Defs.WellKnown;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs.Epinephrine;

public sealed class JobDriver_UseEpinephrine : JobDriver_UseMedicalDrug
{
    public const string JOB_LABEL_KEY = "MI_UseEpinephrine";

    protected override ThingDef DeviceDef => KnownThingDefOf.Epinephrine;

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device) =>
        new JobDescriptor(KnownJobDefOf.UseEpinephrine, doctor, patient, device);
}
