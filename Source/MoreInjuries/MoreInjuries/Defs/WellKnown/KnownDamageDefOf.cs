using MoreInjuries.Integrations;
using RimWorld;
using Verse;

namespace MoreInjuries.Defs.WellKnown;

[DefOf]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_DEF_OF_REQUIRES_FIELD)]
public static class KnownDamageDefOf
{
    // CE stuff
    [DefAlias("Thermobaric")]
    [MayRequire(SupportedMods.COMBAT_EXTENDED)]
    public static DamageDef? CE_Thermobaric = null;
}
