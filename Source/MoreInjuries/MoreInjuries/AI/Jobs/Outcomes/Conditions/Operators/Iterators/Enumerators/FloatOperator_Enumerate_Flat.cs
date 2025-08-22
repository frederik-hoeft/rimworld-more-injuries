using System.Collections.Generic;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Iterators.Enumerators;

public abstract class FloatOperator_Enumerate_Flat : FloatOperator_Enumerate
{
    protected abstract IEnumerable<float> FlatEnumerate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState);

    public override IEnumerable<FloatOperator> Enumerate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        using FloatOperator_PooledValue box = FloatOperator_PooledValue.Rent();
        foreach (float element in FlatEnumerate(doctor, patient, device, runtimeState))
        {
            box.Value = element;
            yield return box;
        }
    }
}
