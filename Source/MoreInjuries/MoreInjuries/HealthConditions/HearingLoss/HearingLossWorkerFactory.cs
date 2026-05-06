using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.HearingLoss;

[XmlSerializable]
public sealed partial class HearingLossWorkerFactory : IInjuryWorkerFactory
{
    [XmlMember("earGroups")]
    public partial IReadOnlyList<BodyPartGroupDef> EarGroups { get; }

    public InjuryWorker Create(MoreInjuryComp parent) => new HearingLossWorker(parent, EarGroups);
}
