using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes;

[XmlBindable]
public partial class JobOutcomeProperties_ModExtension : DefModExtension
{
    private static readonly List<JobOutcomeDoer> s_emptyOutcomeDoers = [];

    [XmlBinding("outcomeDoers", DefaultValueFrom = nameof(s_emptyOutcomeDoers))]
    public virtual partial IReadOnlyList<JobOutcomeDoer> OutcomeDoers { get; }
}
