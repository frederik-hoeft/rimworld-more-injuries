using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Binary;

[XmlSerializable]
public abstract partial class FloatOperator_Binary : FloatOperator
{
    protected abstract string OperatorSymbol { get; }

    [XmlMember("left")]
    public partial FloatOperator Left { get; internal set; }

    [XmlMember("right")]
    public partial FloatOperator Right { get; internal set; }

    public override string ToString() => $"({Left} {OperatorSymbol} {Right})";
}
