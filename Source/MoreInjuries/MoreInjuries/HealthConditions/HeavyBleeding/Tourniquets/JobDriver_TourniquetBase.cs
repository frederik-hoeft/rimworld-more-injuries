using MoreInjuries.AI;
using System.Runtime.CompilerServices;
using Verse.AI;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding.Tourniquets;

public abstract class JobDriver_TourniquetBase : JobDriver_UseMedicalDevice
{
    internal static readonly ConditionalWeakTable<Job, BodyPartRecord> s_transientTargetParts = new();
    protected BodyPartRecord? _bodyPart;

    public override void Notify_Starting()
    {
        base.Notify_Starting();
        if(s_transientTargetParts.TryGetValue(job, out BodyPartRecord? bodyPart))
        {
            _bodyPart = bodyPart;
            s_transientJobParameters.Remove(job);
        }
    }

    protected override int BaseTendDuration => 90;

    protected override bool IsTreatable(Hediff hediff) => true;
}
