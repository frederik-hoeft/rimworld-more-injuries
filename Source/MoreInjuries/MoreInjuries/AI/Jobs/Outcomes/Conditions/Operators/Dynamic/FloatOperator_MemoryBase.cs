using MoreInjuries.Roslyn.Future.ThrowHelpers;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

// memebers initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
[SuppressMessage(CODE_STYLE, STYLE_IDE0032_USE_AUTO_PROPERTY, Justification = JUSTIFY_IDE0032_XML_DEF_REQUIRES_FIELD)]
public abstract class FloatOperator_MemoryBase(string? symbol) : FloatOperator
{
    // don't rename this field. XML defs depend on this name
    // can't use primary constructor member here, since it will get a compiler-generated name
    private readonly string? symbol = symbol;

    protected string Symbol
    {
        get
        {
            Throw.InvalidOperationException.IfNullOrEmpty(this, symbol);
            return symbol;
        }
    }
}
