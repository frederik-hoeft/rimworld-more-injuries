using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Ternary;

[XmlBindable]
public sealed partial class FloatOperator_Ternary : FloatOperator
{
    [XmlBinding("condition")]
    public partial OutcomeDoerCondition Condition { get; }

    [XmlBinding("whenTrue")]
    public partial FloatOperator WhenTrue { get; }

    [XmlBinding("whenFalse")]
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
