using MoreInjuries.BuildIntrinsics;
using MoreInjuries.Extensions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;

// members initialized via XML defs
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class BodyPartHediffTargetEvaluator_RandomFromList : BodyPartHediffTargetEvaluator
{
    // don't rename this field. XML defs depend on this name
    private readonly List<BodyPartDef> targets = default!;

    public override BodyPartRecord? GetTargetBodyPart(HediffComp comp, HediffCompHandler_SecondaryCondition handler)
    {
        if (targets is not { Count: > 0 })
        {
            Logger.Error($"{nameof(BodyPartHediffTargetEvaluator_RandomFromList)}: {comp.GetType().Name} has no target defined. Cannot evaluate.");
            return null;
        }
        BodyPartRecord? target = comp.Pawn.health.hediffSet.GetNotMissingParts()
            .Where(part => targets.Contains(part.def))
            .ToList()
            .SelectRandom();
        return target;
    }
}
