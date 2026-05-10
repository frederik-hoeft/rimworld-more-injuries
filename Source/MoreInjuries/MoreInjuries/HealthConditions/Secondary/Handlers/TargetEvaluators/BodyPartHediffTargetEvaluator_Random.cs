using MoreInjuries.Extensions.Bcl;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;

[XmlBindable]
public partial class BodyPartHediffTargetEvaluator_Random : BodyPartHediffTargetEvaluator
{
    [XmlBinding<BodyPartHeight>("height", defaultValue: BodyPartHeight.Undefined)]
    public partial BodyPartHeight Height { get; }

    [XmlBinding<BodyPartDepth>("depth", defaultValue: BodyPartDepth.Undefined)]
    public partial BodyPartDepth Depth { get; }

    [XmlBinding("excludedParts")]
    public partial List<BodyPartDef>? ExcludedParts { get; }

    [XmlBinding("includedParts")]
    public partial List<BodyPartDef>? IncludedParts { get; }

    protected virtual bool IncludeBodyPart(BodyPartRecord bodyPart, Pawn pawn) =>
        // include all body parts that are not excluded
        ExcludedParts?.Contains(bodyPart.def) is not true;

    public override BodyPartRecord? GetTargetBodyPart(HediffComp comp, HediffCompHandler_SecondaryCondition handler)
    {
        Pawn pawn = comp.Pawn;
        IEnumerable<BodyPartRecord> bodyParts = pawn.health.hediffSet.GetNotMissingParts(Height, Depth);
        bodyParts = bodyParts.Where(bodyPart => IncludeBodyPart(bodyPart, pawn));
        if (IncludedParts is [_, ..] includedParts)
        {
            IEnumerable<BodyPartRecord> allBodyParts = pawn.health.hediffSet.GetNotMissingParts();
            bodyParts = bodyParts.Union(allBodyParts.Where(bodyPart => includedParts.Contains(bodyPart.def)));
        }
        return bodyParts.ToList().SelectRandomOrDefault();
    }
}
