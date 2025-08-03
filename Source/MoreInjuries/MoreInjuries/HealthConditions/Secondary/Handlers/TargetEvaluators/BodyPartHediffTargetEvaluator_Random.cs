using MoreInjuries.Extensions;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public class BodyPartHediffTargetEvaluator_Random : BodyPartHediffTargetEvaluator
{
    // don't rename this field. XML defs depend on this name
    private readonly BodyPartHeight height = BodyPartHeight.Undefined;
    // don't rename this field. XML defs depend on this name
    private readonly BodyPartDepth depth = BodyPartDepth.Undefined;
    // don't rename this field. XML defs depend on this name
    private readonly List<BodyPartDef>? excludedParts = null;
    // don't rename this field. XML defs depend on this name
    private readonly List<BodyPartDef>? includedParts = null;

    protected virtual bool IncludeBodyPart(BodyPartRecord bodyPart, Pawn pawn) =>
        // include all body parts that are not excluded
        excludedParts is not { Count: > 0 } || !excludedParts.Contains(bodyPart.def);

    public override BodyPartRecord? GetTargetBodyPart(HediffComp comp, HediffCompHandler_SecondaryCondition handler)
    {
        Pawn pawn = comp.Pawn;
        IEnumerable<BodyPartRecord> bodyParts = pawn.health.hediffSet.GetNotMissingParts(height, depth);
        bodyParts = bodyParts.Where(bodyPart => IncludeBodyPart(bodyPart, pawn));
        if (includedParts is { Count: > 0 })
        {
            IEnumerable<BodyPartRecord> allBodyParts = pawn.health.hediffSet.GetNotMissingParts();
            bodyParts = bodyParts.Union(allBodyParts.Where(bodyPart => includedParts.Contains(bodyPart.def)));
        }
        return bodyParts.ToList().SelectRandomOrDefault();
    }
}
