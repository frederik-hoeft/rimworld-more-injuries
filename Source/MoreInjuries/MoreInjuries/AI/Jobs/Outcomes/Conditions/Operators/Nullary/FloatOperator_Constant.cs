using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Nullary;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class FloatOperator_Constant : FloatOperator
{
    // don't rename this field. XML defs depend on this name
    private readonly float value = default;

    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState) => value;
}
