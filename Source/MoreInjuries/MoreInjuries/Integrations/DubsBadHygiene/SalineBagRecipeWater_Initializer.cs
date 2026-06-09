using DubsBadHygiene;
using MoreInjuries.Initialization;
using MoreInjuries.Logging;
using System;
using Verse;

namespace MoreInjuries.Integrations.DubsBadHygiene;

/// <summary>
/// When Dubs Bad Hygiene is installed, the saline bag recipes require a <c>DBH_WaterBottle</c> ingredient
/// (added via <c>MayRequire</c> in <c>Defs/RecipeDefs/Things/Medicine_SalineBag.xml</c>). Dubs Bad Hygiene
/// ships with its Thirst need disabled by default, and when Thirst is off the game cannot produce water
/// bottles, so the recipes become permanently uncraftable. This initializer detects that configuration at
/// load time and removes the water bottle ingredient so the saline bags can still be crafted from stone
/// blocks alone. See https://github.com/frederik-hoeft/rimworld-more-injuries/issues/150.
/// </summary>
[StaticConstructorOnStartup]
public static class SalineBagRecipeWater_Initializer
{
    private const string WaterBottleDefName = "DBH_WaterBottle";

    private static readonly string[] s_salineBagRecipeDefNames =
    [
        "Make_SalineBag",
        "Make_SalineBagFive",
    ];

    static SalineBagRecipeWater_Initializer()
    {
        if (!ModsConfig.IsActive(SupportedMods.DUBS_BAD_HYGIENE))
        {
            // The integration assembly is compiled in, but Dubs Bad Hygiene is not actually active in this
            // playthrough. The recipes carry no DBH_WaterBottle ingredient (it is added via MayRequire), so
            // there is nothing to prune.
            return;
        }

        if (IsThirstNeedEnabled())
        {
            // Thirst is active, so water bottles can be produced and the recipes are craftable as authored.
            return;
        }

        if (DefDatabase<ThingDef>.GetNamedSilentFail(WaterBottleDefName) is not { } waterBottle)
        {
            return;
        }

        foreach (string recipeDefName in s_salineBagRecipeDefNames)
        {
            if (DefDatabase<RecipeDef>.GetNamedSilentFail(recipeDefName) is not { } recipe)
            {
                continue;
            }

            if (RecipeIngredientPruner.RemoveIngredient(recipe, waterBottle))
            {
                Logger.Log($"Removed '{WaterBottleDefName}' from recipe '{recipeDefName}' because the Dubs Bad Hygiene Thirst need is disabled.");
            }
        }
    }

    /// <summary>
    /// Reads the Dubs Bad Hygiene Thirst setting. Returns <see langword="true"/> (leave the recipes
    /// untouched) if the setting cannot be read, so that any future change to Dubs Bad Hygiene's settings
    /// API degrades to the unmodified, vanilla-compatible behaviour instead of silently breaking the recipe.
    /// </summary>
    private static bool IsThirstNeedEnabled()
    {
        try
        {
            Settings settings = LoadedModManager.GetMod<DubsBadHygieneMod>().GetSettings<Settings>();
            return settings.ThirstNeed;
        }
        catch (Exception exception)
        {
            Logger.Warning($"Could not read the Dubs Bad Hygiene Thirst setting; leaving saline bag recipes unchanged. ({exception.Message})");
            return true;
        }
    }
}
