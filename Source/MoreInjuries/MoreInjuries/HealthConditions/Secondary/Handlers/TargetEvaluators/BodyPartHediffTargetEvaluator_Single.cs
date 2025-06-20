using MoreInjuries.BuildIntrinsics;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;

// members initialized via XML defs
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public sealed class BodyPartHediffTargetEvaluator_Single : BodyPartHediffTargetEvaluator
{
    // don't rename this field. XML defs depend on this name
    private readonly BodyPartDef target = default!;

    public override BodyPartRecord? GetTargetBodyPart(HediffComp comp, HediffCompHandler_SecondaryCondition handler)
    {
        if (target is null)
        {
            Logger.Error($"{nameof(BodyPartHediffTargetEvaluator_Single)}: {comp.GetType().Name} has no target defined. Cannot evaluate.");
            return null;
        }
        HediffSet hediffs = comp.Pawn.health.hediffSet;
        if (hediffs.GetBodyPartRecord(target) is BodyPartRecord targetRecord && !hediffs.PartIsMissing(targetRecord))
        {
            return targetRecord;
        }
        return null;
    }
}