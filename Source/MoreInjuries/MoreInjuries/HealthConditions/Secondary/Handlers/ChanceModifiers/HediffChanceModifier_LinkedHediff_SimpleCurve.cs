using MoreInjuries.BuildIntrinsics;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.ChanceModifiers;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class HediffChanceModifier_LinkedHediff_SimpleCurve : SecondaryHediffChanceModifier
{
    // don't rename this field. XML defs depend on this name
    private readonly SimpleCurve severityCurve = default!;
    // don't rename this field. XML defs depend on this name
    private readonly HediffDef hediffDef = default!;

    /// <inheritdoc />
    public override float GetModifier(HediffComp_SecondaryCondition comp, HediffCompHandler_SecondaryCondition compHandler)
    {
        if (severityCurve is null || hediffDef is null)
        {
            Logger.ConfigError($"{nameof(HediffChanceModifier_LinkedHediff_SimpleCurve)} is not properly initialized. Current severity curve is null. Cannot evaluate chance.");
            return 1f;
        }
        if (comp.parent.pawn.health.hediffSet.TryGetHediff(hediffDef, out Hediff? hediff))
        {
            // if the hediff exists, we evaluate the chance based on the severity curve
            return severityCurve.Evaluate(hediff.Severity);
        }
        // if the hediff does not exist, we return the base chance
        return 1f;
    }
}