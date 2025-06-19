using MoreInjuries.BuildIntrinsics;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.ChanceModifiers;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public class HediffChanceModifier_LinkedHediffCurve : SecondaryHediffChanceModifier
{
    // don't rename this field. XML defs depend on this name
    private readonly SimpleCurve severityCurve = default!;
    // don't rename this field. XML defs depend on this name
    private readonly HediffDef hediffDef = default!;

    /// <inheritdoc />
    public override float GetModifer(Pawn pawn)
    {
        if (severityCurve is null || hediffDef is null)
        {
            string errorMessage = $"Hediff chance modifier {GetType().Name} is not properly initialized. Cannot evaluate chance.";
            Logger.Error(errorMessage);
#if DEBUG
            throw new InvalidOperationException(errorMessage);
#endif
            return 1f;
        }
        if (pawn.health.hediffSet.TryGetHediff(hediffDef, out Hediff? hediff))
        {
            // if the hediff exists, we evaluate the chance based on the severity curve
            return severityCurve.Evaluate(hediff.Severity);
        }
        // if the hediff does not exist, we return the base chance
        return 1f;
    }
}
