using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators;
using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Nullary;
using MoreInjuries.Defs.WellKnown;
using MoreInjuries.Roslyn.Future.ThrowHelpers;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.Hemodilution;

/// <summary>
/// Calculates the hemodilution level based on the current hemodilution severity, blood loss severity, and the two required parameters:
/// <list type="bullet">
/// <item><description><c>added_volume</c> - The volume of blood products or saline added to the patient.</description></item>
/// <item><description><c>dilution_factor</c> - The dilution factor, which is <c>+1.0</c> for saline (increases hemodilution) and <c>-1.0</c> for blood products (decreases hemodilution).</description></item>
/// </list>
/// </summary>
public sealed class DynamicProcedureCall_CalculateHemodilution : FloatOperator
{
    public const string PARAMETER_ADDED_VOLUME = "added_volume";
    public const string PARAMETER_DILUTION_FACTOR = "dilution_factor";

    private static readonly Lazy<FloatOperator_HediffSeverity> s_getHemodilutionSeverity = new
    (
        valueFactory: static () => new FloatOperator_HediffSeverity(KnownHediffDefOf.Hemodilution),
        isThreadSafe: false
    );
    private static readonly Lazy<FloatOperator_HediffSeverity> s_getBloodLossSeverity = new
    (
        valueFactory: static () => new FloatOperator_HediffSeverity(HediffDefOf.BloodLoss),
        isThreadSafe: false
    );

    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        Throw.ArgumentNullException.IfNull(runtimeState);
        float addedVolume = runtimeState.ResolveRequired(PARAMETER_ADDED_VOLUME);
        float dilutionFactor = runtimeState.ResolveRequired(PARAMETER_DILUTION_FACTOR);
        float hemodilution = s_getHemodilutionSeverity.Value.Evaluate(doctor, patient, device, runtimeState);
        float bloodLoss = s_getBloodLossSeverity.Value.Evaluate(doctor, patient, device, runtimeState);
        return HemodilutionEvaluator.CalculateHemodilutionDelta(hemodilution, bloodLoss, addedVolume, dilutionFactor);
    }

    public override string ToString() => $"calculate_hemodilution_delta({PARAMETER_ADDED_VOLUME}, {PARAMETER_DILUTION_FACTOR})";
}
