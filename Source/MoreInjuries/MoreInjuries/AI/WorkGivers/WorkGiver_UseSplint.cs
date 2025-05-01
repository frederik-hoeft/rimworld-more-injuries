using MoreInjuries.HealthConditions.Fractures;
using MoreInjuries.KnownDefs;
using MoreInjuries.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace MoreInjuries.AI.WorkGivers;

public class WorkGiver_UseSplint : WorkGiver_MoreInjuriesTreatmentBase
{
    protected override bool CanTreat(Hediff hediff) => Array.IndexOf(JobDriver_UseSplint.TargetHediffDefs, hediff.def) != -1;

    protected override bool IsValidPatient(Pawn doctor, Thing thing, out Pawn patient) => 
        base.IsValidPatient(doctor, thing, out patient) 
        && patient.playerSettings?.medCare is not MedicalCareCategory.NoCare and not MedicalCareCategory.NoMeds;

    protected override bool CanTreat(Pawn doctor, Pawn patient) => 
        MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.Splint, JobDriver_UseSplint.TargetHediffDefs) is not null 
        && patient.BillStack?.Bills?.Find(b => b.recipe == KnownRecipeDefOf.SplintFracture || b.recipe == KnownRecipeDefOf.RepairFracture) is null
        && base.CanTreat(doctor, patient);

    public override bool ShouldSkip(Pawn pawn, bool forced = false) =>
        !KnownResearchProjectDefOf.BasicAnatomy.IsFinished
        || !MoreInjuriesMod.Settings.EnableApplySplintJob;

    protected override Job CreateJob(Pawn doctor, Pawn patient) => MedicalDeviceHelper.FindMedicalDevice(doctor, patient, KnownThingDefOf.Splint, JobDriver_UseSplint.TargetHediffDefs) is Thing splint
        ? JobDriver_UseSplint.GetDispatcher(doctor, patient, splint).CreateJob()
        : GetDummyDefaultJob(doctor);
}
