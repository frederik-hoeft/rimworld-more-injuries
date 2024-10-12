using MoreInjuries.AI;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

public abstract class JobDriver_TourniquetBase : JobDriver_UseMedicalDevice
{
    protected string? _bodyPartWoundAnchorTag;

    public override void Notify_Starting()
    {
        base.Notify_Starting();

        if (Parameters is TourniquetBaseParameters { woundAnchorTag: string woundAnchorTag })
        {
            _bodyPartWoundAnchorTag = woundAnchorTag;
        }
    }

    protected override int BaseTendDuration => 90;

    // we always either apply or remove exactly one tourniquet
    protected override int GetMedicalDeviceCountToFullyHeal(Pawn patient) => 1;

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Matches XML node naming.")]
    protected class TourniquetBaseParameters : ExtendedJobParameters
    {
        public string? woundAnchorTag;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref woundAnchorTag, nameof(woundAnchorTag));
        }
    }
}
