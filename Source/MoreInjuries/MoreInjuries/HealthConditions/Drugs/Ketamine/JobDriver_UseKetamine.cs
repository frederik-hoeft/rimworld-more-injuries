using MoreInjuries.AI.Jobs;
using MoreInjuries.Defs.WellKnown;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs.Ketamine;

public sealed class JobDriver_UseKetamine : JobDriver_UseMedicalDrug
{
    public const string JOB_LABEL_KEY = "MI_UseKetamine";

    protected override ThingDef DeviceDef => KnownThingDefOf.Ketamine;

    protected override bool MustReservePatient(Pawn doctor, Pawn patient) => false;

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device) =>
        new JobDescriptor(KnownJobDefOf.UseKetamine, doctor, patient, device);
}