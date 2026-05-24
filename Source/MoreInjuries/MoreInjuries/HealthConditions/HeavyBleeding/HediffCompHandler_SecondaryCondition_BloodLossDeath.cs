using HarmonyLib;
using MoreInjuries.HealthConditions.Secondary;
using MoreInjuries.HealthConditions.Secondary.Handlers;
using MoreInjuries.HealthConditions.Secondary.Handlers.HediffMakers;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding;

public sealed class HediffCompHandler_SecondaryCondition_BloodLossDeath : HediffCompHandler_SecondaryCondition_Tick
{
    // TODO: add Roslyn analyzer to check if this field is still valid after each RimWorld update
    private static readonly AccessTools.FieldRef<Hediff, float> s_severityInt = AccessTools.FieldRefAccess<Hediff, float>("severityInt");

    internal static void Apply(Hediff bloodLoss)
    {
        // kill pawn by bypassing maxSeverity of blood loss (1), lethal severity is patched to be 1.01f
        float value = bloodLoss.def.lethalSeverity;
        ref float severityInt = ref s_severityInt(bloodLoss);
        severityInt = value;
        bloodLoss.pawn.health.Notify_HediffChanged(bloodLoss);
    }

    protected override void Evaluate(HediffComp_SecondaryCondition comp, HediffMakerDef hediffMakerDef) => Apply(comp.parent);
}
