using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.Initialization;

/// <summary>
/// Helpers for removing ingredients from a <see cref="RecipeDef"/> at load time when the thing they
/// require cannot be produced in the current mod configuration (for example, a Dubs Bad Hygiene water
/// bottle when the Thirst need is disabled). Kept free of any third-party-mod types so the logic can be
/// compiled and unit-tested in every build configuration, while the conditional gating that decides
/// <em>when</em> to call it lives in the mod-specific integration code.
/// </summary>
internal static class RecipeIngredientPruner
{
    /// <summary>
    /// Removes every ingredient line of <paramref name="recipe"/> whose filter resolves to nothing but
    /// the given <paramref name="thingDef"/>, and drops that def from the recipe's fixed and default
    /// ingredient filters. Ingredient lines that also accept other things (for example a line that allows
    /// any stone block) are left untouched. The method is idempotent: running it again after the def has
    /// already been removed is a no-op.
    /// </summary>
    /// <returns><see langword="true"/> if anything was changed, otherwise <see langword="false"/>.</returns>
    public static bool RemoveIngredient(RecipeDef recipe, ThingDef thingDef)
    {
        if (recipe is null || thingDef is null)
        {
            return false;
        }

        bool changed = false;

        if (recipe.ingredients is { } ingredients)
        {
            // Iterate over a snapshot so we can mutate the backing list while deciding what to drop.
            foreach (IngredientCount ingredient in ingredients.ToList())
            {
                if (ResolvesOnlyTo(ingredient, thingDef))
                {
                    ingredients.Remove(ingredient);
                    changed = true;
                }
            }
        }

        changed |= Disallow(recipe.fixedIngredientFilter, thingDef);
        changed |= Disallow(recipe.defaultIngredientFilter, thingDef);

        return changed;
    }

    private static bool ResolvesOnlyTo(IngredientCount? ingredient, ThingDef thingDef)
    {
        if (ingredient?.filter is not { } filter)
        {
            return false;
        }

        List<ThingDef> allowed = filter.AllowedThingDefs?.ToList() ?? [];
        // Only drop the line if it exists solely to require the target def. A line that also accepts other
        // things (a category such as StoneBlocks, or a multi-thing filter) must survive so the recipe keeps
        // its remaining ingredients.
        return allowed.Count > 0 && allowed.All(def => def == thingDef);
    }

    private static bool Disallow(ThingFilter? filter, ThingDef thingDef)
    {
        if (filter is null || !filter.Allows(thingDef))
        {
            return false;
        }

        filter.SetAllow(thingDef, false);
        return true;
    }
}
