using MoreInjuries.Extensions.Bcl;
using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.TargetEvaluators;

[XmlSerializable]
public partial class BodyPartHediffTargetEvaluator_RandomFromList : BodyPartHediffTargetEvaluator
{
    [XmlMember("targets")]
    public partial IReadOnlyList<BodyPartDef> Targets { get; }

    public override BodyPartRecord? GetTargetBodyPart(HediffComp comp, HediffCompHandler_SecondaryCondition handler)
    {
        if (Targets is not { Count: > 0 } targets)
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

