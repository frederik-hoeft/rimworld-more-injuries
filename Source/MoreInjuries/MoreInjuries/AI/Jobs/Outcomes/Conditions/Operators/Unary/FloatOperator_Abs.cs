using UnityEngine;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Unary;

public sealed class FloatOperator_Abs : FloatOperator_Unary
{
    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState) => 
        Mathf.Abs(Inner.Evaluate(doctor, patient, device, runtimeState));
}