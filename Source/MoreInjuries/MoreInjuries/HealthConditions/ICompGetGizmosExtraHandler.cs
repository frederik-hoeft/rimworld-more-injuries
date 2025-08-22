using Verse;

namespace MoreInjuries.HealthConditions;

public interface ICompGetGizmosExtraHandler : IInjuryHandler
{
    void AddGizmosExtra(UIBuilder<Gizmo> builder);
}
