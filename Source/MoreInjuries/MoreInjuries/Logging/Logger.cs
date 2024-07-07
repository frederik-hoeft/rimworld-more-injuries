namespace MoreInjuries.Logging;

internal static class Logger
{
    public static void Log(string message)
    {
        if (MoreInjuriesMod.Settings.enableLogging)
        {
            Verse.Log.Message($"[{nameof(MoreInjuries)}] {message}");
        }
    }

    public static void LogVerbose(string message)
    {
        if (MoreInjuriesMod.Settings.enableVerboseLogging)
        {
            Log(message);
        }
    }

    public static void Error(string message) =>
        Verse.Log.Error($"[{nameof(MoreInjuries)}] {message}");

    public static void LogAlways(string message) =>
        Verse.Log.Message($"[{nameof(MoreInjuries)}] {message}");
}
