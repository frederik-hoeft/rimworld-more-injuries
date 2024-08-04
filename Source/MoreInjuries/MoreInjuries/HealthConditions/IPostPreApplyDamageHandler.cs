using Verse;

namespace MoreInjuries.HealthConditions;

public interface IPostPreApplyDamageHandler : IInjuryHandler
{
    void PostPreApplyDamage(ref readonly DamageInfo dinfo);
}
