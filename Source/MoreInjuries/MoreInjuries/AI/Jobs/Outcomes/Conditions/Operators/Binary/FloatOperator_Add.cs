using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Binary;

public sealed class FloatOperator_Add : FloatOperator_Binary
{
    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState) =>
        Left.Evaluate(doctor, patient, device, runtimeState) + Right.Evaluate(doctor, patient, device, runtimeState);
}
