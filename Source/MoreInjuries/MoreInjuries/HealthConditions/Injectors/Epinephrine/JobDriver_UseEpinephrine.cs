using MoreInjuries.AI;
using MoreInjuries.KnownDefs;
using Verse;

namespace MoreInjuries.HealthConditions.Injectors.Epinephrine;

public class JobDriver_UseEpinephrine : JobDriver_UseInjector_GivesHediff
{
    public const string JOB_LABEL_KEY = "MI_UseEpinephrine";

    protected override HediffDef HediffDef => KnownHediffDefOf.AdrenalineRush;

    protected override ThingDef DeviceDef => KnownThingDefOf.Epinephrine;

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device) =>
        new JobDescriptor(KnownJobDefOf.UseEpinephrine, doctor, patient, device);
}
