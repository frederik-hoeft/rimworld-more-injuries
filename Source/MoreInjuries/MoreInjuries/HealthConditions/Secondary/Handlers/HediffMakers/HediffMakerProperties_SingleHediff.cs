using MoreInjuries.Roslyn.SourceGen.XmlSerialization.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;

[XmlSerializable]
public partial class HediffMakerProperties_SingleHediff : HediffMakerProperties
{
    [XmlMember("hediffMakerDef")]
    private partial HediffMakerDef HediffMakerDef { get; }

    public override HediffMakerDef GetHediffMakerDef(HediffComp parentComp, HediffCompHandler_SecondaryCondition handler, BodyPartRecord? targetBodyPart) => HediffMakerDef;
}
