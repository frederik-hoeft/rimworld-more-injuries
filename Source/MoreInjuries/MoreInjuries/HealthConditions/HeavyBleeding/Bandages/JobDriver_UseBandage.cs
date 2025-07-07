using MoreInjuries.AI.Jobs;
using MoreInjuries.Defs.WellKnown;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Bandages;

public class JobDriver_UseBandage : JobDriver_HemostasisBase
{
    public const string JOB_LABEL_KEY = "MI_UseBandages";

    protected override ThingDef DeviceDef => KnownThingDefOf.Bandage;

    protected override int BaseTendDuration => 180;

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device, bool fromInventoryOnly) =>
        GetDispatcher(KnownJobDefOf.UseBandage, doctor, patient, device, fromInventoryOnly);
}