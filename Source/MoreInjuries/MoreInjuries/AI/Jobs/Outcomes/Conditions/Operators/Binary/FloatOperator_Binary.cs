using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Binary;

[XmlBindable]
public abstract partial class FloatOperator_Binary : FloatOperator
{
    protected abstract string OperatorSymbol { get; }

    [XmlBinding("left")]
    public partial FloatOperator Left { get; internal set; }

    [XmlBinding("right")]
    public partial FloatOperator Right { get; internal set; }

    public override string ToString() => $"({Left} {OperatorSymbol} {Right})";
}
