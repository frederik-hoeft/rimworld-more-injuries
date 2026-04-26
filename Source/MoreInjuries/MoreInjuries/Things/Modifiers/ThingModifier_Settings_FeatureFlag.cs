using Verse;

namespace MoreInjuries.Things.Modifiers;

public class ThingModifier_Settings_FeatureFlag : ThingModifier_Settings
{
    public override float GetModifier(Thing thing)
    {
        string feature = Key;
        if (!MoreInjuriesMod.Settings.Keyed.TryGetMember(feature, out bool flag))
        {
            // if the feature flag does not exist or does not match the expected type, we return the base chance
            Logger.ConfigError($"{feature} is not a valid feature flag in the settings. Cannot evaluate chance.");
            return 1f;
        }
        if (!flag)
        {
            // if the feature flag is disabled, we return 0 chance
            return 0f;
        }
        // if the feature flag is enabled, we return the base chance
        return 1f;
    }
}
