using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.HearingLoss;

[XmlBindable]
public sealed partial class HearingLossWorkerFactory : IInjuryWorkerFactory
{
    [XmlBinding("earGroups")]
    public partial IReadOnlyList<BodyPartGroupDef> EarGroups { get; }

    public InjuryWorker Create(MoreInjuryComp parent) => new HearingLossWorker(parent, EarGroups);
}
