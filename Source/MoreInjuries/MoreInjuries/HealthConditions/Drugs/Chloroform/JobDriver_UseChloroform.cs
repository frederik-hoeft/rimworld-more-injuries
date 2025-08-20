using MoreInjuries.AI.Audio;
using MoreInjuries.AI.Jobs;
using MoreInjuries.Defs.WellKnown;
using Verse;

namespace MoreInjuries.HealthConditions.Drugs.Chloroform;

public sealed class JobDriver_UseChloroform : JobDriver_UseMedicalDrug
{
    private static readonly ChloroformSoundDefProvider s_soundDefProvider = new();
    public const string JOB_LABEL_KEY = "MI_UseChloroform";

    protected override ThingDef DeviceDef => KnownThingDefOf.Chloroform;

    protected override ISoundDefProvider<Pawn> SoundDefProvider => s_soundDefProvider;

    protected override int BaseTendDuration => 120;

    protected override bool MustReservePatient(Pawn doctor, Pawn patient) => false;

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device) =>
        new JobDescriptor(KnownJobDefOf.UseChloroform, doctor, patient, device);
}