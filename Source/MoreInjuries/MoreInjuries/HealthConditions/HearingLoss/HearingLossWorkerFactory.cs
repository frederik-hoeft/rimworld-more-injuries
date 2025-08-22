using MoreInjuries.Roslyn.Future.ThrowHelpers;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.HearingLoss;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class HearingLossWorkerFactory : IInjuryWorkerFactory
{
    // don't rename this field. XML defs depend on this name
    private readonly List<BodyPartGroupDef>? earGroups = default;

    public InjuryWorker Create(MoreInjuryComp parent)
    {
        List<BodyPartGroupDef> ears = Throw.InvalidOperationException.IfNull(this, earGroups);
        return new HearingLossWorker(parent, ears);
    }
}
