using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

[XmlSerializable]
public sealed partial class FloatOperator_AssignConstant : FloatOperator_AssignBase
{
    [XmlMember("value")]
    public partial float Value { get; init; }

    public FloatOperator_AssignConstant() : base(null) { }

    internal FloatOperator_AssignConstant(string symbol, float value) : base(symbol)
    {
        Value = value;
    }

    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState) => AssignValue(Value, runtimeState);

    protected override string ValueToString() => Value.ToString();
}
