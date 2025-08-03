using MoreInjuries.Roslyn.Future.ThrowHelpers;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class BodyPartHediffTargetEvaluator_Single : BodyPartHediffTargetEvaluator
{
    // don't rename this field. XML defs depend on this name
    private readonly BodyPartDef target = default!;

    public override BodyPartRecord? GetTargetBodyPart(HediffComp comp, HediffCompHandler_SecondaryCondition handler)
    {
        Throw.InvalidOperationException.IfNull(this, target);
        HediffSet hediffs = comp.Pawn.health.hediffSet;
        if (hediffs.GetBodyPartRecord(target) is BodyPartRecord targetRecord && !hediffs.PartIsMissing(targetRecord))
        {
            return targetRecord;
        }
        return null;
    }
}