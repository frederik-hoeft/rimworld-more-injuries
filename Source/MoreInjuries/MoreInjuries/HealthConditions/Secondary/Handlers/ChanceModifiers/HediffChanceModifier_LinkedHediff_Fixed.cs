using MoreInjuries.BuildIntrinsics;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.ChanceModifiers;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class HediffChanceModifier_LinkedHediff_Fixed : SecondaryHediffChanceModifier
{
    // don't rename this field. XML defs depend on this name
    private readonly HediffDef hediffDef = default!;
    // don't rename this field. XML defs depend on this name
    private readonly float chanceModifier = -1f;

    /// <inheritdoc />
    public override float GetModifier(HediffComp_SecondaryCondition comp, HediffCompHandler_SecondaryCondition compHandler)
    {
        if (chanceModifier < 0f || hediffDef is null)
        {
            Logger.ConfigError($"{nameof(HediffChanceModifier_LinkedHediff_SimpleCurve)} is not properly initialized. Current severity curve is null. Cannot evaluate chance.");
            return 1f;
        }
        if (comp.parent.pawn.health.hediffSet.HasHediff(hediffDef))
        {
            // if the hediff exists, we evaluate the chance based on the severity curve
            return chanceModifier;
        }
        // if the hediff does not exist, we return the base chance
        return 1f;
    }
}