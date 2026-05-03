using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

[XmlSerializable]
public sealed partial class FloatOperator_Assign : FloatOperator_AssignBase
{
    [XmlMember("value")]
    public partial FloatOperator Value { get; init; }

    public FloatOperator_Assign() : base(null) { }

    internal FloatOperator_Assign(string symbol, FloatOperator value) : base(symbol)
    {
        Value = value;
    }

    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        float evaluatedValue = Value.Evaluate(doctor, patient, device, runtimeState);
        return AssignValue(evaluatedValue, runtimeState);
    }

    protected override string ValueToString() => Value.ToString();
}
