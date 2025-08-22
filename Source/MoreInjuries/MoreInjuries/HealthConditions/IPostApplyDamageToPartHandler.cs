using Verse;

namespace MoreInjuries.HealthConditions;

public interface IPostApplyDamageToPartHandler : IInjuryHandler
{
    void ApplyDamageToPart(ref readonly DamageInfo dinfo, Pawn pawn, DamageWorker.DamageResult result);
}