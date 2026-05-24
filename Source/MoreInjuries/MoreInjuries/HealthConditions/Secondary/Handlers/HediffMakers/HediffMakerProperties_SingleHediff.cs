using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;

[XmlBindable]
public partial class HediffMakerProperties_SingleHediff : HediffMakerProperties
{
    [XmlBinding("hediffMakerDef")]
    private partial HediffMakerDef HediffMakerDef { get; }

    public override HediffMakerDef GetHediffMakerDef(HediffComp parentComp, HediffCompHandler_SecondaryCondition handler) => HediffMakerDef;
}
