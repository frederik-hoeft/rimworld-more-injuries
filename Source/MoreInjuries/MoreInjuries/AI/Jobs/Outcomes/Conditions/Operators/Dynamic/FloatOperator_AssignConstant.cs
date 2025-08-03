using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

// memebers initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class FloatOperator_AssignConstant : FloatOperator_AssignBase
{
    // don't rename this field. XML defs depend on this name
    private readonly float value = default;

    public FloatOperator_AssignConstant() : base(null) { }

    internal FloatOperator_AssignConstant(string symbol, float value) : base(symbol)
    {
        this.value = value;
    }

    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState) => AssignValue(value, runtimeState);

    protected override string ValueToString() => value.ToString();
}
