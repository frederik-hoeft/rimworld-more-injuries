using MoreInjuries.Extensions;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public class BodyPartHediffTargetEvaluator_RandomFromList : BodyPartHediffTargetEvaluator
{
    // don't rename this field. XML defs depend on this name
    private readonly List<BodyPartDef> targets = default!;

    public override BodyPartRecord? GetTargetBodyPart(HediffComp comp, HediffCompHandler_SecondaryCondition handler)
    {
        if (targets is not { Count: > 0 })
        {
            Logger.Error($"{comp.GetType().Name} has no target defined. Cannot evaluate.");
            return null;
        }
        BodyPartRecord? target = comp.Pawn.health.hediffSet.GetNotMissingParts()
            .Where(part => targets.Contains(part.def))
            .ToList()
            .SelectRandomOrDefault();
        return target;
    }
}

