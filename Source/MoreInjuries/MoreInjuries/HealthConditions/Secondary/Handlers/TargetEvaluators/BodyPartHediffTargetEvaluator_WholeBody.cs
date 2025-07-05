using System.Threading;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;

public sealed class BodyPartHediffTargetEvaluator_WholeBody : BodyPartHediffTargetEvaluator
{
    private static BodyPartHediffTargetEvaluator_WholeBody? s_instance;

    public static BodyPartHediffTargetEvaluator_WholeBody Instance
    {
        get
        {
            if (Volatile.Read(ref s_instance) is null)
            {
                Interlocked.CompareExchange(ref s_instance, value: new BodyPartHediffTargetEvaluator_WholeBody(), comparand: null);
            }
            return Volatile.Read(ref s_instance)!;
        }
    }

    public override BodyPartRecord? GetTargetBodyPart(HediffComp comp, HediffCompHandler_SecondaryCondition handler) => null;
}