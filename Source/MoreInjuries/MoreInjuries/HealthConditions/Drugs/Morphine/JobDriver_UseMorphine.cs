using MoreInjuries.AI;
using MoreInjuries.KnownDefs;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs.Morphine;

public sealed class JobDriver_UseMorphine : JobDriver_UseMedicalDrug
{
    public const string JOB_LABEL_KEY = "MI_UseMorphine";

    protected override ThingDef DeviceDef => KnownThingDefOf.Morphine;

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device) =>
        new JobDescriptor(KnownJobDefOf.UseMorphine, doctor, patient, device);
}
