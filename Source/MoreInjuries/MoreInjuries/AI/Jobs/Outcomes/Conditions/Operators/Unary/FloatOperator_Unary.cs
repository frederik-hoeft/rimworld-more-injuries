namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Unary;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
public abstract class FloatOperator_Unary : FloatOperator
{
    // don't rename this field. XML defs depend on this name
    private readonly FloatOperator? inner = default;

    public FloatOperator Inner => inner ?? throw new InvalidOperationException("Inner operator is not set. This should never happen.");
}