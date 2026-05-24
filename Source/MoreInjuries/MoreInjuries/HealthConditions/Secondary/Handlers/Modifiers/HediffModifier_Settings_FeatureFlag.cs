using Verse;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.Modifiers;

public class HediffModifier_Settings_FeatureFlag : HediffModifier_Settings
{
    public override float GetModifier(Hediff hediff, IHediffCompHandler compHandler)
    {
        string feature = Key;
        if (!MoreInjuriesMod.Settings.Keyed.TryGetMember(feature, out bool flag))
        {
            // if the feature flag does not exist or does not match the expected type, we return the base chance
            Logger.ConfigError($"{feature} is not a valid feature flag in the settings. Cannot evaluate chance.");
            return NoChange;
        }
        if (!flag)
        {
            return Disallow;
        }
        return NoChange;
    }
}
