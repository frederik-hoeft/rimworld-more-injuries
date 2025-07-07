using UnityEngine;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Binary;

public sealed class FloatOperator_Divide : FloatOperator_Binary
{
    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        float rightValue = Right.Evaluate(doctor, patient, device, runtimeState);
        if (Mathf.Approximately(rightValue, 0f))
        {
            throw new DivideByZeroException($"{nameof(FloatOperator_Divide)}: Division by (approximately) zero is not allowed. Right operand value was {rightValue}.");
        }
        return Left.Evaluate(doctor, patient, device, runtimeState) / rightValue;
    }
}
