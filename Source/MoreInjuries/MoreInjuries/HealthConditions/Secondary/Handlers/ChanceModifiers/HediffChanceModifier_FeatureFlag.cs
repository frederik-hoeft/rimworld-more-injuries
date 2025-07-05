using MoreInjuries.BuildIntrinsics;

namespace MoreInjuries.HealthConditions.Secondary.Handlers.ChanceModifiers;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.XML_NAMING_CONVENTION)]
public sealed class HediffChanceModifier_FeatureFlag : SecondaryHediffChanceModifier
{
    // don't rename this field. XML defs depend on this name
    private readonly string? key = null;

    public override float GetModifier(HediffComp_SecondaryCondition comp, HediffCompHandler_SecondaryCondition compHandler)
    {
        string? feature = key;
        if (string.IsNullOrEmpty(feature))
        {
            // if the feature flag does not exist, we return the base chance
            Logger.ConfigError($"{nameof(HediffChanceModifier_FeatureFlag)} is missing the key. Cannot evaluate chance.");
            return 1f;
        }
        if (!MoreInjuriesMod.Settings.Keyed.TryGetMember(feature!, out bool flag))
        {
            // if the feature flag does not exist or does not match the expected type, we return the base chance
            Logger.ConfigError($"{nameof(HediffChanceModifier_FeatureFlag)}: {feature} is not a valid feature flag in the settings. Cannot evaluate chance.");
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