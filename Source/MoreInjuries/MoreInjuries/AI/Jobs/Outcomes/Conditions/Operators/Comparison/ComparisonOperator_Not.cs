namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Comparison;

// members initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class ComparisonOperator_Not : ComparisonOperator
{
    // don't rename this field. XML defs depend on this name
    private readonly ComparisonOperator? inner = default;

    public override bool Compare(float left, float right)
    {
        _ = inner ?? throw new InvalidOperationException($"{nameof(ComparisonOperator_Not)} requires an inner operator to function correctly. Please check your XML definition.");
        return !inner.Compare(left, right);
    }
}