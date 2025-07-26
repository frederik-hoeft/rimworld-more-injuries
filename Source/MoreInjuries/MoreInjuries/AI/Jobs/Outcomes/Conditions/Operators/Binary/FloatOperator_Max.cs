using UnityEngine;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Binary;

public sealed class FloatOperator_Max : FloatOperator_Binary_Function
{
    protected override string OperatorSymbol => "max";

    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState) =>
        Mathf.Max(Left.Evaluate(doctor, patient, device, runtimeState), Right.Evaluate(doctor, patient, device, runtimeState));
}