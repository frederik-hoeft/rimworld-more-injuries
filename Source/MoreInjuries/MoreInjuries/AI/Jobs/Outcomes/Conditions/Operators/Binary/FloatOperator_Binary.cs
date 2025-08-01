using MoreInjuries.Roslyn.Future.ThrowHelpers;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Binary;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
public abstract class FloatOperator_Binary : FloatOperator
{
    // don't rename this field. XML defs depend on this name
    private FloatOperator? left = default;
    // don't rename this field. XML defs depend on this name
    private FloatOperator? right = default;

    protected abstract string OperatorSymbol { get; }

    public FloatOperator Left
    {
        get
        {
            Throw.InvalidOperationException.IfNull(this, left);
            return left;
        }
        internal set => left = value;
    }

    public FloatOperator Right
    {
        get
        {
            Throw.InvalidOperationException.IfNull(this, right);
            return right;
        }
        internal set => right = value;
    }

    public override string ToString() => $"({Left} {OperatorSymbol} {Right})";
}