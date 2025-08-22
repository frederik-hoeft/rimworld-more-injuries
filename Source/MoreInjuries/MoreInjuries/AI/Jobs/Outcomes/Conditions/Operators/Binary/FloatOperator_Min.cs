using UnityEngine;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Binary;

public sealed class FloatOperator_Min : FloatOperator_Binary_Function
{
    protected override string OperatorSymbol => "min";

    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState) =>
        Mathf.Min(Left.Evaluate(doctor, patient, device, runtimeState), Right.Evaluate(doctor, patient, device, runtimeState));
}