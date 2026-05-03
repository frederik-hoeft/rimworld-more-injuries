using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Ternary;

[XmlSerializable]
public sealed partial class FloatOperator_Ternary : FloatOperator
{
    [XmlMember("condition")]
    public partial OutcomeDoerCondition Condition { get; }

    [XmlMember("whenTrue")]
    public partial FloatOperator WhenTrue { get; }

    [XmlMember("whenFalse")]
    public partial FloatOperator WhenFalse { get; }

    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        if (Condition.ShouldDoOutcome(doctor, patient, device, runtimeState))
        {
            return WhenTrue.Evaluate(doctor, patient, device, runtimeState);
        }
        return WhenFalse.Evaluate(doctor, patient, device, runtimeState);
    }

    public override string ToString() => $"({Condition} ? {WhenTrue} : {WhenFalse})";
}
