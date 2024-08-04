using Verse;

namespace MoreInjuries.HealthConditions;

public interface IPostPostApplyDamageHandler : IInjuryHandler
{
    void PostPostApplyDamage(ref readonly DamageInfo dinfo);
}
