using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Binary;
using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Iterators.Enumerators;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Iterators;

[XmlBindable]
public sealed partial class FloatOperator_Accumulate : FloatOperator
{
    private readonly object _accumulationLock = new();

    [XmlBinding("accumulationFunction")]
    public partial FloatOperator_Binary AccumulationFunction { get; }

    [XmlBinding("enumerable")]
    public partial FloatOperator_Enumerate Enumerable { get; }

    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        // we are modifying the accumulationFunction, so we need to lock it
        lock (_accumulationLock)
        {
            FloatOperator_Binary op = AccumulationFunction;
            FloatOperator_Enumerate enumerable = Enumerable;
            using FloatOperator_PooledValue acc = FloatOperator_PooledValue.Rent();
            bool hasValue = false;
            op.Left = acc;
            foreach (FloatOperator element in enumerable.Enumerate(doctor, patient, device, runtimeState))
            {
                if (!hasValue)
                {
                    acc.Value = element.Evaluate(doctor, patient, device, runtimeState);
                    hasValue = true;
                    continue;
                }
                op.Right = element;
                acc.Value = op.Evaluate(doctor, patient, device, runtimeState);
            }
            op.Left = null!;
            op.Right = null!;
            return acc.Value; // either 0 or the accumulated value
        }
    }

    public override string ToString() => $"accumulate({AccumulationFunction}, {Enumerable})";
}
