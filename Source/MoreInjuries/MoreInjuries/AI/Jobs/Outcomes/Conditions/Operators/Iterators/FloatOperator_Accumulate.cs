using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Binary;
using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Iterators.Enumerators;
using MoreInjuries.Roslyn.Future.ThrowHelpers;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Iterators;

// memebers initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class FloatOperator_Accumulate : FloatOperator
{
    // don't rename this field. XML defs depend on this name
    private readonly FloatOperator_Binary? accumulationFunction = default;
    // don't rename this field. XML defs depend on this name
    private readonly FloatOperator_Enumerate? enumerable = default;

    private readonly object _accumulationLock = new();

    public FloatOperator_Binary AccumulationFunction
    {
        get
        {
            Throw.InvalidOperationException.IfNull(this, accumulationFunction);
            return accumulationFunction;
        }
    }

    public FloatOperator_Enumerate Enumerable
    {
        get
        {
            Throw.InvalidOperationException.IfNull(this, enumerable);
            return enumerable;
        }
    }

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

    public override string ToString() => $"accumulate({accumulationFunction?.ToString() ?? "null"}, {Enumerable?.ToString() ?? "null"})";
}