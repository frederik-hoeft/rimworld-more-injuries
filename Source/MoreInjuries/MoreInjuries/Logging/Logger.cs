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

    public static void Warning(string message) =>
        Verse.Log.Warning($"[{nameof(MoreInjuries)}] {message}");

    public static void LogAlways(string message) =>
        Verse.Log.Message($"[{nameof(MoreInjuries)}] {message}");

#if DEBUG
    [DoesNotReturn]
#endif
    public static void ConfigError(string message)
    {
        string formattedMessage = $"[{nameof(MoreInjuries)}] Config Error: {message}";
        Verse.Log.Error(formattedMessage);
#if DEBUG
        throw new InvalidOperationException(formattedMessage);
#endif
    }
}
