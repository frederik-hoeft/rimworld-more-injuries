using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public sealed class HediffModifier_Settings_Gauge : HediffModifier_Settings
{
    public override float GetModifier(Hediff hediff, IHediffCompHandler compHandler)
    {
        string gaugeKey = Key;
        if (!MoreInjuriesMod.Settings.Keyed.TryGetMember(gaugeKey, out float gauge))
        {
            // if the key does not exist or does not match the expected type, we return the base chance
            Logger.ConfigError($"{gaugeKey} is not a valid key in the settings. Cannot evaluate chance.");
            return NoChange;
        }
        // if there is a valid gauge value associated with the key, we return it
        return gauge;
    }
}
