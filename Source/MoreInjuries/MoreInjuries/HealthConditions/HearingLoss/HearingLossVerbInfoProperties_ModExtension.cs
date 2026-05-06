using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.HealthConditions.HearingLoss;

[XmlBindable]
public sealed partial class HearingLossVerbInfoProperties_ModExtension : DefModExtension
{
    [XmlBinding("supportedVerbBaseClasses")]
    public partial IReadOnlyList<string> SupportedVerbBaseClasses { get; }
}
