using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators;

public abstract class FloatOperator
{
    public abstract float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState);

    public abstract override string ToString();
}