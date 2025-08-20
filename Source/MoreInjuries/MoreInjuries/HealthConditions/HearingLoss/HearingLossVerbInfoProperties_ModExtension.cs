using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.HearingLoss;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
public sealed class HearingLossVerbInfoProperties_ModExtension : DefModExtension
{
    // don't rename this field. XML defs depend on this name
    private readonly List<string>? supportedVerbBaseClasses = default;

    public List<string> SupportedVerbBaseClasses => supportedVerbBaseClasses ?? [];
}