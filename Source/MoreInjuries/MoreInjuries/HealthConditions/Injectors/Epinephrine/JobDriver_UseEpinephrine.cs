using MoreInjuries.AI;
using MoreInjuries.KnownDefs;
using Verse;

namespace MoreInjuries.HealthConditions.Injectors.Epinephrine;

public sealed class JobDriver_UseEpinephrine : JobDriver_UseInjector
{
    public const string JOB_LABEL_KEY = "MI_UseEpinephrine";

    protected override ThingDef DeviceDef => KnownThingDefOf.Epinephrine;

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device) =>
        new JobDescriptor(KnownJobDefOf.UseEpinephrine, doctor, patient, device);
}
