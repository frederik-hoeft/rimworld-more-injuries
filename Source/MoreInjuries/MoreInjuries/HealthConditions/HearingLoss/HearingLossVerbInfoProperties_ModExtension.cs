using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.HearingLoss;

[XmlSerializable]
public sealed partial class HearingLossVerbInfoProperties_ModExtension : DefModExtension
{
    [XmlMember("supportedVerbBaseClasses")]
    public partial IReadOnlyList<string> SupportedVerbBaseClasses { get; }
}
