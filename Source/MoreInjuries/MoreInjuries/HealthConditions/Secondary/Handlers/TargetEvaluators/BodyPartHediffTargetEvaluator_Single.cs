using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;

[XmlSerializable]
public sealed partial class BodyPartHediffTargetEvaluator_Single : BodyPartHediffTargetEvaluator
{
    [XmlMember("target")]
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
