using MoreInjuries.AI.Audio;
using MoreInjuries.AI.Jobs;
using MoreInjuries.Caching;
using MoreInjuries.Defs.WellKnown;
using RimWorld;
using Verse;
using Verse.AI;

using static MoreInjuries.HealthConditions.HeavyBleeding.BloodLossConstants;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

public class JobDriver_HarvestBlood : JobDriver_UseMedicalDevice
{
    public const string JOB_LABEL_KEY = "MI_HarvestBlood";
    private static readonly WeakTimedDataCache<Pawn, float, TimedDataEntry<float>> s_pawnBloodLossCache = new
    (
        minCacheRefreshIntervalTicks: GenTicks.TicksPerRealSecond,
        dataProvider: static pawn =>
        {
            if (pawn.health.hediffSet.TryGetHediff(HediffDefOf.BloodLoss, out Hediff? bloodLoss))
            {
                return bloodLoss.Severity;
            }
            return 0f;
        }
    );

    protected override int BaseTendDuration => 720;

    protected override bool RequiresDevice => false;

    protected override ISoundDefProvider<Pawn> SoundDefProvider => CachedSoundDefProvider.Of<Pawn>(SoundDefOf.Recipe_Surgery);

    protected override ThingDef DeviceDef => null!;

    protected override bool IsTreatable(Hediff hediff) => true;

    // this runs on every tick, so only sample blood loss every second
    protected override bool RequiresTreatment(Pawn patient) => JobCanTreat(patient);

    public static bool JobCanTreat(Pawn patient, bool bypassCache = false) => !patient.Dead && s_pawnBloodLossCache.GetData(patient, bypassCache) < 1f;

    protected override bool ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        if (!patient.health.hediffSet.TryGetHediff(HediffDefOf.BloodLoss, out Hediff bloodLoss))
        {
            bloodLoss = HediffMaker.MakeHediff(HediffDefOf.BloodLoss, patient);
            bloodLoss.Severity = BLOOD_LOSS_THRESHOLD;
            patient.health.AddHediff(bloodLoss);
        }
        else
        {
            float newSeverity = bloodLoss.Severity + BLOOD_LOSS_THRESHOLD;
            if (newSeverity >= 1f && !patient.Dead)
            {
                // if the severity is over 100% we kill the pawn immediately
                // (by bumping it over the max severity to the lethal severity)
                HediffCompHandler_SecondaryCondition_BloodLossDeath.Apply(bloodLoss);
            }
            else
            {
                bloodLoss.Severity = newSeverity;
            }
        }
        if (!GenPlace.TryPlaceThing(ThingMaker.MakeThing(JobDriver_UseBloodBag.JobDeviceDef), patient.PositionHeld, patient.MapHeld, ThingPlaceMode.Near))
        {
            Logger.Error($"Could not drop blood bag near {patient.PositionHeld}");
        }
        if (patient.Faction is Faction factionToInform && (factionToInform != Faction.OfPlayer || patient.IsQuestLodger()))
        {
            Faction.OfPlayer.TryAffectGoodwillWith(factionToInform, goodwillChange: -50, canSendHostilityLetter: !factionToInform.temporary, reason: KnownHistoryEventDefOf.ExtractedWholeBloodBag);
            QuestUtility.SendQuestTargetSignals(patient.questTags, QuestUtility.QuestTargetSignalPart_SurgeryViolation, patient.Named("SUBJECT"));
        }
        if (patient.Dead)
        {
            ThoughtUtility.GiveThoughtsForPawnExecuted(patient, doctor, PawnExecutionKind.OrganHarvesting);
            TaleRecorder.RecordTale(TaleDefOf.ExecutedPrisoner, doctor, patient);
            if (doctor.needs?.mood?.thoughts?.memories is not null)
            {
                Thought_Memory thought = (Thought_Memory)ThoughtMaker.MakeThought(KnownThoughtDefOf.HarvestedBlood_Bloodlust);
                doctor.needs.mood.thoughts.memories.TryGainMemory(thought);
            }
        }
        // force-invalidate the cache
        s_pawnBloodLossCache.MarkDirty(patient);
        return true;
    }

    public static IJobDescriptor GetDispatcher(Pawn doctor, Pawn patient) =>
        new JobDescriptor(doctor, patient);

    protected class JobDescriptor(Pawn doctor, Pawn patient) : IJobDescriptor
    {
        public Job CreateJob()
        {
            Job job = JobMaker.MakeJob(KnownJobDefOf.HarvestBlood, patient);
            job.count = 1;
            return job;
        }

        public void StartJob()
        {
            Job job = CreateJob();
            doctor.jobs.TryTakeOrderedJob(job);
        }
    }
}
