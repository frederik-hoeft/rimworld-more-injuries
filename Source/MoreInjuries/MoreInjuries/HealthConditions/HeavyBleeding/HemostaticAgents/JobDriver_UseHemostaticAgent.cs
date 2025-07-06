using MoreInjuries.AI;
using MoreInjuries.Defs.WellKnown;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.HemostaticAgents;

public class JobDriver_UseHemostaticAgent : JobDriver_HemostasisBase
{
    public const string JOB_LABEL_KEY = "MI_UseHemostaticAgent";

    protected override ThingDef DeviceDef => KnownThingDefOf.HemostaticAgent;

    protected override int BaseTendDuration => 60;

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient, Thing device, bool fromInventoryOnly) => 
        GetDispatcher(KnownJobDefOf.UseHemostaticAgent, doctor, patient, device, fromInventoryOnly);
}