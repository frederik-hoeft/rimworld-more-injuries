using Verse;

namespace MoreInjuries.HealthConditions;

public interface ICompFloatMenuOptionsHandler : IInjuryHandler
{
    void AddFloatMenuOptions(UIBuilder<FloatMenuOption> builder, Pawn selectedPawn);
}
