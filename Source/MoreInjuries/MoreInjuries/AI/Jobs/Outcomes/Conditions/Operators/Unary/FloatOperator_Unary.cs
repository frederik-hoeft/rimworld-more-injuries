using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Unary;

[XmlSerializable]
public abstract partial class FloatOperator_Unary : FloatOperator
{
    [XmlMember("inner")]
    public partial FloatOperator Inner { get; }
}
