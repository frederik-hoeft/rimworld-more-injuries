namespace MoreInjuries.HealthConditions.Secondary.Handlers;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public class HediffCompHandler_SecondaryCondition_PostMake : HediffCompHandler_SecondaryCondition, IHediffComp_SecondaryCondition_PostMakeHandler
{
    public virtual void PostMake(HediffComp_SecondaryCondition comp)
    {
        if (!ShouldSkip(comp))
        {
            // Evaluate the comp immediately after it is created
            Evaulate(comp);
        }
    }
}
