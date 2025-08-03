namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Binary;

public abstract class FloatOperator_Binary_Function : FloatOperator_Binary
{
    public override string ToString() => $"{OperatorSymbol}({Left}, {Right})";
}