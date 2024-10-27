using MoreInjuries.AI;
using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

using static BloodLossConstants;

public class JobDriver_TransfuseBlood : JobDriver_UseMedicalDevice
{
    private const float BLOOD_LOSS_HEALED_PER_PACK = 0.35f;

    protected override bool RequiresDevice => true;

    protected override ThingDef DeviceDef => KnownThingDefOf.WholeBloodBag;

    protected override SoundDef SoundDef => base.SoundDef;

    protected override int BaseTendDuration => 720;

    protected override int GetMedicalDeviceCountToFullyHeal(Pawn patient)
    {
        if (patient.health.hediffSet.TryGetHediff(HediffDefOf.BloodLoss, out Hediff bloodLoss))
        {
            return (int)(bloodLoss.Severity / BLOOD_LOSS_HEALED_PER_PACK);
        }
        return 0;
    }

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {

    }

    protected override bool IsTreatable(Hediff hediff) => hediff.def == HediffDefOf.BloodLoss && hediff.Severity > BLOOD_LOSS_HEALED_PER_PACK;

    protected override bool RequiresTreatment(Pawn patient) => GetMedicalDeviceCountToFullyHeal(patient) > 0;
}
