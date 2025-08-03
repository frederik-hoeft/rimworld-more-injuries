using System.Collections.Generic;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Iterators.Enumerators;

public abstract class FloatOperator_Enumerate : Symbol
{
    public abstract IEnumerable<FloatOperator> Enumerate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState);
}
