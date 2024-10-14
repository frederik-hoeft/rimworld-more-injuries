using MoreInjuries.AI;
using System.Diagnostics.CodeAnalysis;
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

    // we always either apply or remove exactly one tourniquet
    protected override int GetMedicalDeviceCountToFullyHeal(Pawn patient) => 1;

    protected static string? GetUniqueBodyPartKey(BodyPartRecord? bodyPart) => bodyPart?.woundAnchorTag ?? bodyPart?.def.defName;

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
