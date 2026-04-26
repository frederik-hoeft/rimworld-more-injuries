using Verse;

namespace MoreInjuries.Things.Modifiers;

public sealed class ThingModifier_Settings_Gauge : ThingModifier_Settings
{
    public override float GetModifier(Thing thing)
    {
        string feature = Key;
        if (!MoreInjuriesMod.Settings.Keyed.TryGetMember(feature, out float gauge))
        {
            // if the key does not exist or does not match the expected type, we return the base chance
            Logger.ConfigError($"{feature} is not a valid feature flag in the settings. Cannot evaluate chance.");
            return 1f;
        }
        // if there is a valid gauge value associated with the key, we return it
        return gauge;
    }
}
