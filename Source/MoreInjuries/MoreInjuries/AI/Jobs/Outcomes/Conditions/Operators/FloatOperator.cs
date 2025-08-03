using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators;

public abstract class FloatOperator : Symbol
{
    public abstract float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState);
}