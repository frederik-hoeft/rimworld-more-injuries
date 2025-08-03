using MoreInjuries.Roslyn.Future.ThrowHelpers;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

// memebers initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class FloatOperator_Assign : FloatOperator_AssignBase
{
    // don't rename this field. XML defs depend on this name
    private readonly FloatOperator? value = default;

    public FloatOperator_Assign() : base(null) { }

    internal FloatOperator_Assign(string symbol, FloatOperator value) : base(symbol)
    {
        this.value = value;
    }

    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        Throw.InvalidOperationException.IfNull(this, value);
        float evaluatedValue = value.Evaluate(doctor, patient, device, runtimeState);
        return AssignValue(evaluatedValue, runtimeState);
    }

    protected override string ValueToString() => value?.ToString() ?? "null";
}
