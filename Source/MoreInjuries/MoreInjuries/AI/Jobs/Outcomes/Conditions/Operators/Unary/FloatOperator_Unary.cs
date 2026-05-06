using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Unary;

[XmlBindable]
public abstract partial class FloatOperator_Unary : FloatOperator
{
    [XmlBinding("inner")]
    public partial FloatOperator Inner { get; }
}
