using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions;

public interface ICompFloatMenuOptionsHandler : IInjuryHandler
{
    IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selectedPawn);
}
