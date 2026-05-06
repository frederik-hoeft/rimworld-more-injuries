using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

[XmlSerializable]
public sealed partial class HediffModifierReference_ModExtension : DefModExtension
{
    [XmlMember("modifier")]
    public partial SecondaryHediffModifier Modifier { get; }
}
