using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

[XmlBindable]
public sealed partial class DynamicRuntimeProcedureDef_ModExtension : DefModExtension
{
    [XmlBinding("instructions")]
    public partial List<FloatOperator>? Instructions { get; }
}
