using MoreInjuries.AI;
using MoreInjuries.AI.Audio;
using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;

using static BloodLossConstants;

public class JobDriver_HarvestBlood : JobDriver_UseMedicalDevice
{
    public const string JOB_LABEL_KEY = "MI_HarvestBlood";

    protected override int BaseTendDuration => 720;

    protected override bool RequiresDevice => false;

    protected override ISoundDefProvider<Pawn> SoundDefProvider => CachedSoundDefProvider.Of<Pawn>(SoundDefOf.Recipe_Surgery);

    protected override ThingDef DeviceDef => null!;

    protected override bool IsTreatable(Hediff hediff) => true;

    protected override bool RequiresTreatment(Pawn patient) => !patient.Dead;

    protected override void ApplyDevice(Pawn doctor, Pawn patient, Thing? device)
    {
        if (!patient.health.hediffSet.TryGetHediff(HediffDefOf.BloodLoss, out Hediff hediff))
        {
            hediff = HediffMaker.MakeHediff(HediffDefOf.BloodLoss, patient);
            hediff.Severity = 0.001f;
            patient.health.AddHediff(hediff);
        }
        hediff.Severity += BLOOD_LOSS_THRESHOLD;
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
