using System.Threading;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;

public sealed class BodyPartHediffTargetEvaluator_WholeBody : BodyPartHediffTargetEvaluator
{
    public static BodyPartHediffTargetEvaluator_WholeBody Instance
    {
        get
        {
            if (Volatile.Read(ref field) is null)
            {
                Interlocked.CompareExchange(ref field, value: new BodyPartHediffTargetEvaluator_WholeBody(), comparand: null);
            }
            return Volatile.Read(ref field)!;
        }
    }

    public override BodyPartRecord? GetTargetBodyPart(HediffComp comp, HediffCompHandler_SecondaryCondition handler) => null;
}
