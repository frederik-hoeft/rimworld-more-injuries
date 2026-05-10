using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

[XmlBindable]
public abstract partial class FloatOperator_MemoryBase(string? defaultSymbol) : FloatOperator
{
    [XmlBinding("symbol", DefaultValueFrom = nameof(defaultSymbol))]
    protected partial string Symbol { get; }
}
