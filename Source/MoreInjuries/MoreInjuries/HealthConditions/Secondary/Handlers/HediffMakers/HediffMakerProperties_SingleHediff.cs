using MoreInjuries.Roslyn.Future.ThrowHelpers;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;

[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public class HediffMakerProperties_SingleHediff : HediffMakerProperties
{
    // don't rename this field. XML defs depend on this name
    private readonly HediffMakerDef hediffMakerDef = default!;

    public override HediffMakerDef GetHediffMakerDef(HediffComp parentComp, HediffCompHandler_SecondaryCondition handler, BodyPartRecord? targetBodyPart)
    {
        Throw.InvalidOperationException.IfNull(this, hediffMakerDef);
        return hediffMakerDef;
    }
}
