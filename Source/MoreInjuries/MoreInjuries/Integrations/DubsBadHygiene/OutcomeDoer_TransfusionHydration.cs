#if MOD_BAD_HYGIENE

using DubsBadHygiene;
using MoreInjuries.AI.Jobs.Outcomes;
using MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;
using RimWorld;
using Verse;

namespace MoreInjuries.Integrations.DubsBadHygiene;

public sealed class OutcomeDoer_TransfusionHydration : JobOutcomeDoer_NeedBase
{
    protected override bool DoOutcome(Pawn doctor, Pawn patient, Thing? device, Need need)
    {
        if (need is not Need_Thirst thirst)
        {
            Logger.Error($"Unexpected need type {need.GetType()}. Expected {typeof(Need_Thirst)}");
            return false;
        }
        if (device?.def.GetModExtension<TransfusionProperties_ModExtension>() is not { } transfusionProps)
        {
            Logger.ConfigError($"Ran hydration outcome doer with unsupported device '{device?.def.defName ?? "null"}' (missing mod extension)");
            return true;
        }
        thirst.Drink(2f * transfusionProps.BloodLossSeverityReduction);
        return true;
    }
}
#endif