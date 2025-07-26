using MoreInjuries.AI.Jobs;
using MoreInjuries.Defs.WellKnown;
using MoreInjuries.HealthConditions.CardiacArrest;
using MoreInjuries.HealthConditions.Choking;
using MoreInjuries.HealthConditions.HeavyBleeding;
using MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;
using MoreInjuries.Things;
using Verse;

namespace MoreInjuries.HealthConditions;

internal sealed class ProvideFirstAidWorker(MoreInjuryComp parent) : InjuryWorker(parent), ICompFloatMenuOptionsHandler
{
    public override bool IsEnabled => true;

    public void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn)
    {
        Pawn patient = Target;
        if (patient != selectedPawn && selectedPawn.Drafted && !builder.Keys.Contains(UITreatmentOption.ProvideFirstAid))
        {
            builder.Keys.Add(UITreatmentOption.ProvideFirstAid);
            if (!KnownResearchProjectDefOf.BasicFirstAid.IsFinished)
            {
                return;
            }
            if (MedicalDeviceHelper.GetCauseForDisabledProcedure(selectedPawn, patient, string.Empty) is not null)
            {
                return;
            }
            bool canTreat = false;
            // blood bag usage possible => saling bag usage may be possible as well (no need to check)
            if (JobDriver_UseBloodBag.JobGetMedicalDeviceCountToFullyHeal(patient, fullyHeal: false) > 0)
            {
                canTreat = true;
            }
            else
            {
                // check if we can treat any of the known conditions
                foreach (Hediff hediff in patient.health.hediffSet.hediffs)
                {
                    if (JobDriver_HemostasisBase.JobCanTreat(hediff) 
                        || KnownResearchProjectDefOf.EmergencyMedicine.IsFinished 
                            && (JobDriver_UseDefibrillator.JobCanTreat(hediff)
                            || Array.IndexOf(JobDriver_UseSuctionDevice.TargetHediffDefs, hediff.def) != -1
                            || Array.IndexOf(JobDriver_PerformCpr.TargetHediffDefs, hediff.def) != -1)
                        || patient.Downed && hediff.TendableNow())
                    {
                        canTreat = true;
                        break;
                    }
                }
            }
            if (canTreat)
            {
                builder.Options.Add(new FloatMenuOption("MI_ProvideFirstAid".Translate(), JobDriver_ProvideFirstAid.GetDispatcher(selectedPawn, patient).StartJob));
            }
        }
    }
}