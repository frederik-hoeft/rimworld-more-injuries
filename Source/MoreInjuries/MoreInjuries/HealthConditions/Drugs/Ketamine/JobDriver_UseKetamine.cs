using MoreInjuries.AI;
using MoreInjuries.KnownDefs;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs.Ketamine;

public sealed class JobDriver_UseKetamine : JobDriver_UseMedicalDrug
{
    public const string JOB_LABEL_KEY = "MI_UseKetamine";

    protected override ThingDef DeviceDef => KnownThingDefOf.Ketamine;

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device) =>
        new JobDescriptor(KnownJobDefOf.UseKetamine, doctor, patient, device);
}