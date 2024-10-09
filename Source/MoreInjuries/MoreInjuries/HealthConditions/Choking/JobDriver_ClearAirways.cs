using MoreInjuries.AI;
using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Choking;

public class JobDriver_ClearAirways : JobDriver_UseMedicalDevice
{
    public static HediffDef[] TargetHediffDefs { get; } = [KnownHediffDefOf.ChokingOnBlood];

    protected override HediffDef[] HediffDefs => TargetHediffDefs;

    protected override ThingDef DeviceDef => KnownThingDefOf.SuctionDevice;

    protected override bool RequiresDevice => true;

    protected override int BaseTendDuration => 600;

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        Hediff? choking = patient.health.hediffSet.hediffs.Find(hediff => hediff.def == KnownHediffDefOf.ChokingOnBlood);
        if (choking is not null && (doctor.skills.GetSkill(SkillDefOf.Medicine).Level >= 5 || Rand.Chance(MoreInjuriesMod.Settings.ChokingSuctionDeviceSuccessRate)))
        {
            patient.health.RemoveHediff(choking);
        }
    }
}
