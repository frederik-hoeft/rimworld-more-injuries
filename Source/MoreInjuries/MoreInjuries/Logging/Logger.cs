using System.Diagnostics;

namespace MoreInjuries.Logging;

internal static class Logger
{
    public static void Log(string message)
    {
        if (MoreInjuriesMod.Settings.EnableLogging)
        {
            Verse.Log.Message($"[{nameof(MoreInjuries)}] {message}");
        }
    }

    public static void LogVerbose(string message)
    {
        if (MoreInjuriesMod.Settings.EnableVerboseLogging)
        {
            Log(message);
        }
    }

    [Conditional("DEBUG")]
    public static void LogDebug(string message)
    {
        Log(message);
    }

    public static void Error(string message) =>
        Verse.Log.Error($"[{nameof(MoreInjuries)}] {message}");

    public static void LogAlways(string message) =>
        Verse.Log.Message($"[{nameof(MoreInjuries)}] {message}");
}
