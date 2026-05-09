namespace MoreInjuries.Extensions;

internal static class SettingsExtensions
{
    public static bool IsLoggingEnabled(this MoreInjuriesSettings settings)
    {
        return settings.EnableLogging;
    }
}
