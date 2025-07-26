using MoreInjuries.Roslyn.Future.ThrowHelpers;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

// memebers initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class FloatOperator_Assign() : FloatOperator
{
    // don't rename this field. XML defs depend on this name
    private readonly string? symbol = default;
    // don't rename this field. XML defs depend on this name
    private readonly FloatOperator? value = default;

    internal FloatOperator_Assign(string symbol, FloatOperator value) : this()
    {
        this.symbol = symbol;
        this.value = value;
    }

    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        Throw.ArgumentNullException.IfNull(runtimeState);
        _ = symbol ?? throw new InvalidOperationException($"{nameof(FloatOperator_Assign)}: cannot assign value to null symbol");
        _ = value ?? throw new InvalidOperationException($"{nameof(FloatOperator_Assign)}: cannot assign null value to symbol {symbol}");
        float evaluatedValue = value.Evaluate(doctor, patient, device, runtimeState);
        runtimeState.Assign(symbol, evaluatedValue);
        return evaluatedValue;
    }

    public override string ToString() => $"({symbol} = {value})";
}
