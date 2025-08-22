using Verse;

namespace MoreInjuries.HealthConditions;

public interface IPostTakeDamageHandler : IInjuryHandler
{
    void PostTakeDamage(DamageWorker.DamageResult damage, ref readonly DamageInfo dinfo);
}