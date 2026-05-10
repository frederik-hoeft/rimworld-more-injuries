using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;

[XmlBindable]
public sealed partial class BodyPartHediffTargetEvaluator_Single : BodyPartHediffTargetEvaluator
{
    [XmlBinding("target")]
    public partial BodyPartDef Target { get; }

    public override BodyPartRecord? GetTargetBodyPart(HediffComp comp, HediffCompHandler_SecondaryCondition handler)
    {
        HediffSet hediffs = comp.Pawn.health.hediffSet;
        if (hediffs.GetBodyPartRecord(Target) is BodyPartRecord targetRecord && !hediffs.PartIsMissing(targetRecord))
        {
            return targetRecord;
        }
        return null;
    }
}
