using MoreInjuries.AI;
using MoreInjuries.Defs.WellKnown;
using RimWorld;
using Verse;
using Verse.AI;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

public abstract class JobDriver_TourniquetBase : JobDriver_UseMedicalDevice
{
    protected string? _bodyPartKey;

    public override void Notify_Starting()
    {
        base.Notify_Starting();

        if (Parameters is TourniquetBaseParameters { bodyPartKey: string bodyPartKey })
        {
            _bodyPartKey = bodyPartKey;
        }
        else
        {
            Logger.Error($"{GetType().Name}: Missing or invalid parameters");
            EndJobWith(JobCondition.Incompletable);
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref _bodyPartKey, "bodyPartKey");
    }

    protected override int BaseTendDuration => 90;

    protected override float BaseExperience => 50f;

    // we always either apply or remove exactly one tourniquet
    protected override int GetMedicalDeviceCountToFullyHeal(Pawn patient) => 1;

    protected static string? GetUniqueBodyPartKey(BodyPartRecord? bodyPart) => bodyPart?.woundAnchorTag ?? bodyPart?.def.defName;

    public static bool PawnKnowsWhatTheyreDoing(Pawn pawn)
    {
        int requiredSkillLevel = 3;
        if (pawn.story.traits.HasTrait(KnownTraitDefOf.SlowLearner))
        {
            requiredSkillLevel += 2;
        }
        Span<SkillRecordTracker> skillRecords =
        [
            new SkillRecordTracker(SkillDefOf.Medicine),
            new SkillRecordTracker(SkillDefOf.Intellectual)
        ];
        foreach (SkillRecord skill in pawn.skills.skills)
        {
            for (int i = 0; i < skillRecords.Length; i++)
            {
                if (skill.def == skillRecords[i].SkillDef && skill.Level < requiredSkillLevel)
                {
                    skillRecords[i].InsufficientSkill = true;
                    // there are only two entries we care about, so we can easily check the other one using some index math
                    if (skillRecords[Math.Abs(i - 1)].InsufficientSkill)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Matches XML node naming.")]
    protected class TourniquetBaseParameters : ExtendedJobParameters
    {
        public string? bodyPartKey;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref bodyPartKey, "bodyPartKey");
        }
    }
}

file struct SkillRecordTracker(SkillDef skillDef)
{
    public readonly SkillDef SkillDef = skillDef;
    public bool InsufficientSkill;
}