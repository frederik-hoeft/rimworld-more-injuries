namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Binary;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
public abstract class FloatOperator_Binary : FloatOperator
{
    // don't rename this field. XML defs depend on this name
    private readonly FloatOperator? left = default;
    // don't rename this field. XML defs depend on this name
    private readonly FloatOperator? right = default;

    protected abstract string OperatorSymbol { get; }

    public FloatOperator Left => left ?? throw new InvalidOperationException($"{nameof(FloatOperator_Binary)}: {nameof(Left)} operand is not set. This should never happen.");

    public FloatOperator Right => right ?? throw new InvalidOperationException($"{nameof(FloatOperator_Binary)}: {nameof(Right)} operand is not set. This should never happen.");

    public override string ToString() => $"({Left} {OperatorSymbol} {Right})";
}