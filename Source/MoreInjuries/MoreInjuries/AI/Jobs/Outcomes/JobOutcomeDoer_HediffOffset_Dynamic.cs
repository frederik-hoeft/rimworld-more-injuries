using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[XmlBindable]
public sealed partial class JobOutcomeDoer_HediffOffset_Dynamic : JobOutcomeDoer_HediffOffsetBase
{
    [XmlBinding("evaluator")]
    public partial FloatOperator Evaluator { get; }

    protected override float GetSeverityOffset(Pawn doctor, Pawn patient, Thing? device) =>
        Evaluator.Evaluate(doctor, patient, device, runtimeState: null);

    public override string ToString() => $"{base.ToString()} with dynamic severity offset evaluator: {Evaluator}";
}
