using MoreInjuries.Extensions;
using MoreInjuries.Localization;
using RimWorld;
using Verse;

namespace MoreInjuries.Things;

public static class ReusabilityUtility
{
    public static void TryReuseSurgeryIngredient(Thing ingredient, Pawn doctor, Pawn patient)
    {
        if (ingredient.def.GetModExtension<ReusabilityProps_ModExtension>() is { DestroyChanceModifier: { } destroyChanceModifier })
        {
            float chance = destroyChanceModifier.GetModifier(ingredient);
            if (Rand.Chance(chance))
            {
                Messages.Message(Named.Keys.Message_ProcedureIngredientDestroyed.Translate(doctor.Named(Named.Params.DOCTOR), ingredient.Named(Named.Params.THING)), doctor, MessageTypeDefOf.NegativeEvent);
            }
            else
            {
                Thing copy = ThingMaker.MakeThing(ingredient.def);
                if (ingredient.def.useHitPoints && ingredient.HitPoints > 0)
                {
                    copy.HitPoints = ingredient.HitPoints;
                }
                if (!GenPlace.TryPlaceThing(copy, patient.PositionHeld, patient.MapHeld, ThingPlaceMode.Near))
                {
                    Logger.Error($"Could not drop {ingredient.ToStringSafe()} near {patient.PositionHeld}");
                }
            }
        }
    }

    public static void TryDestroyReusableIngredient(Thing ingredient, Pawn doctor, bool destroyStack = false)
    {
        if (ingredient.def.GetModExtension<ReusabilityProps_ModExtension>() is { DestroyChanceModifier: { } destroyChanceModifier })
        {
            float chance = destroyChanceModifier.GetModifier(ingredient);
            if (Rand.Chance(chance))
            {
                Messages.Message(Named.Keys.Message_ProcedureIngredientDestroyed.Translate(doctor.Named(Named.Params.DOCTOR), ingredient.Named(Named.Params.THING)), doctor, MessageTypeDefOf.NegativeEvent);
                if (destroyStack)
                {
                    ingredient.Destroy();
                }
                else
                {
                    ingredient.DecreaseStack();
                }
            }
        }
    }
}
