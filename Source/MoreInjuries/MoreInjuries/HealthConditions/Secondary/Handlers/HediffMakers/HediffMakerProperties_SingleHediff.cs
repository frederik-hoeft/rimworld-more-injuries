using MoreInjuries.BuildIntrinsics;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class HediffMakerProperties_SingleHediff : HediffMakerProperties
{
    // don't rename this field. XML defs depend on this name
    private readonly HediffMakerDef hediffMakerDef = default!;

    public override HediffMakerDef GetHediffMakerDef(HediffComp parentComp, HediffCompHandler_SecondaryCondition handler, BodyPartRecord? targetBodyPart) => hediffMakerDef
        ?? throw new InvalidOperationException($"{nameof(HediffMakerProperties_SingleHediff)}: {parentComp.GetType().Name} has no hediff maker defined. Cannot evaluate.");
}
