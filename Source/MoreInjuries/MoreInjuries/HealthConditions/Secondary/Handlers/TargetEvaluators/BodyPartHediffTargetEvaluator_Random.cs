using MoreInjuries.BuildIntrinsics;
using MoreInjuries.Extensions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;

// members initialized via XML defs
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
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

    protected virtual bool IncludeBodyPart(BodyPartRecord bodyPart) =>
        // include all body parts that are not excluded
        excludedParts is null || !excludedParts.Contains(bodyPart.def);

    public override BodyPartRecord? GetTargetBodyPart(HediffComp comp, HediffCompHandler_SecondaryCondition handler)
    {
        IEnumerable<BodyPartRecord> bodyParts = comp.Pawn.health.hediffSet.GetNotMissingParts(height, depth);
        if (excludedParts is { Count: > 0 })
        {
            bodyParts = bodyParts.Where(IncludeBodyPart);
        }
        if (includedParts is { Count: > 0 })
        {
            IEnumerable<BodyPartRecord> allBodyParts = comp.Pawn.health.hediffSet.GetNotMissingParts();
            bodyParts = bodyParts.Union(allBodyParts.Where(bodyPart => includedParts.Contains(bodyPart.def)));
        }
        return bodyParts.ToList().SelectRandomOrDefault();
    }
}
