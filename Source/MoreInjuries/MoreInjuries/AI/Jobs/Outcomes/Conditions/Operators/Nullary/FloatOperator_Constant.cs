using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Nullary;

[XmlSerializable]
public sealed partial class FloatOperator_Constant() : FloatOperator
{
    [XmlMember("value")]
    public partial float Value { get; init; }

    internal FloatOperator_Constant(float value) : this()
    {
        Value = value;
    }

    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState) => Value;

    public override string ToString() => $"const({Value})";
}
