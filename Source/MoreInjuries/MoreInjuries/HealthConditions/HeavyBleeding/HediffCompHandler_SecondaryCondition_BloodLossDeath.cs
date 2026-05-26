using HarmonyLib;
using MoreInjuries.HealthConditions.Secondary;
using MoreInjuries.HealthConditions.Secondary.Handlers;
using MoreInjuries.Roslyn.SourceGen.XmlBinding.Attributes;
using Verse;

namespace MoreInjuries.HealthConditions.HeavyBleeding;

[XmlBindable]
public sealed partial class HediffCompHandler_SecondaryCondition_BloodLossDeath : HediffCompHandler_ChanceBased, IHediffComp_SecondaryCondition_TickHandler
{
    // TODO: add Roslyn analyzer to check if this field is still valid after each RimWorld update
    private static readonly AccessTools.FieldRef<Hediff, float> s_severityInt = AccessTools.FieldRefAccess<Hediff, float>("severityInt");

    [XmlBinding("tickInterval", NullableBackingField = true)]
    public partial int TickInterval { get; }

    internal static void Apply(Hediff bloodLoss)
    {
        // kill pawn by bypassing maxSeverity of blood loss (1), lethal severity is patched to be 1.01f
        float value = bloodLoss.def.lethalSeverity;
        ref float severityInt = ref s_severityInt(bloodLoss);
        severityInt = value;
        bloodLoss.pawn.health.Notify_HediffChanged(bloodLoss);
    }

    public void Handle(HediffComp_SecondaryCondition comp) => Apply(comp.parent);
}
