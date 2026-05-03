using Verse;

namespace MoreInjuries.Things.Modifiers;

public sealed class ThingModifier_Settings_Gauge : ThingModifier_Settings
{
    public override float GetModifier(Thing thing)
    {
        string gaugeKey = Key;
        if (!MoreInjuriesMod.Settings.Keyed.TryGetMember(gaugeKey, out float gauge))
        {
            // if the key does not exist or does not match the expected type, we return the base chance
            Logger.ConfigError($"{gaugeKey} is not a valid key in the settings. Cannot evaluate chance.");
            return 1f;
        }
        // if there is a valid gauge value associated with the key, we return it
        return gauge;
    }
}
